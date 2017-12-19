using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This is the physics engine. It should be attached to an empty game object in
/// the scene. It is invoked via Unity's FixedUpdate function. It manages a set of
/// rigid bodies of type PPhysicsBody. These must be added to the physics engine
/// via a call to AddBody.
/// </summary>
public class PPhysicsEngine : MonoBehaviour {

	private const float groundedDistanceThreshold = 0.1f;

	List<PPhysicsBody> bodies = new List<PPhysicsBody>();

	public void AddBody(PPhysicsBody newBody) {
		Debug.Assert(newBody != null);
		Debug.Assert (bodies!=null);
		bodies.Add (newBody);
	}

	/// <summary>
	/// Determines whether two axis-aligned bounded boxes (rectangles) intersect.
	/// </summary>
	/// <param name="LL1">lower-left corner of first box</param>
	/// <param name="UR1">upper-right corner of first box</param>
	/// <param name="LL2">lower-left corner of second box</param>
	/// <param name="UR2">upper-right corner of second box</param>
	bool Intersecting(Vector2 LL1, Vector2 UR1, Vector2 LL2, Vector2 UR2) {
		return !(UR1.x<LL2.x || UR2.x<LL1.x || UR1.y<LL2.y || UR2.y<LL1.y);
	}

	/// <summary>
	/// Returns true if the two bodies were not intersecting, but now are intersecting
	/// </summary>
	/// <param name="b1">first body</param>
	/// <param name="b2">second body</param>
	bool Colliding(PPhysicsBody b1, PPhysicsBody b2) {
		return !Intersecting (b1.LLold, b1.URold, b2.LLold, b2.URold)
			&& Intersecting(b1.LL, b1.UR, b2.LL, b2.UR);
	}

	/// <summary>
	/// Returns a list of all <i>new</i> collisions. Collisions describe pairs of bodies
	/// that did not collide during the last iteration of the physics engine, but
	/// do collide now.
	/// </summary>
	/// <returns>The for collisions.</returns>
	List<Contact> CheckForCollisions() {
		List<Contact> contacts = new List<Contact>();

		// Compare each pair of bodies to see if they have collided. Be
		// careful to consider each pair only once.
		for(int i=0; i<bodies.Count-1; i++) {
			for(int j = i+1; j<bodies.Count; j++) {
				if(Colliding(bodies[i], bodies[j])) {
					Contact c = new Contact(bodies[i], bodies[j]);
					contacts.Add(c);
				}
			}
		}
		return contacts;
	}

	void IntegrateAll(float deltaTime) {
		foreach(PPhysicsBody pb in bodies) {
			pb.Integrate(deltaTime);
		}
	}

	/// <summary>
	/// Returns true if b is in resting on (or just above) another body. Specifically,
	/// is there another component below this one within the distance
	/// 'groundedDistanceThreshold'.
	/// </summary>
	/// <param name="b">The component we are checking for grounded</param>
	public bool Grounded(PPhysicsBody b) {
		// iterate through all bodies; if b is just above any of them, return true
		foreach(PPhysicsBody b2 in bodies) {
			if(b != b2) {
				if(b.LL.x < b2.UR.x && b.UR.x > b2.LL.x
						&& Mathf.Abs(b.LL.y - b2.UR.y) <= groundedDistanceThreshold) {
					return true;
				}
			}
		} 
		return false;
	}
	
	void ResolveCollisions(List<Contact> contacts) {
		foreach(Contact c in contacts) {
			c.ResolveContact(Time.fixedDeltaTime);
		}
	}
	
	void CommitAll() {
		foreach (PPhysicsBody pb in bodies) {
			pb.Commit();
		}
	}

	/// <summary>
	/// The main method of the physics engine - integrates, checks for collisions,
	/// resolves all collisisons.
	/// </summary>
	void UpdatePhysics() {
        IntegrateAll(Time.fixedDeltaTime);
        List<Contact> collisions = CheckForCollisions();
        ResolveCollisions(collisions);
        CommitAll();
    }


	/// <summary>
	/// On each update tick, run the physic engine - integrate, detect collisions,
	/// respond to collisions
	/// </summary>
	void FixedUpdate () {
		UpdatePhysics();
	}
}

/// <summary>
/// Represents a collision between two rigid bodies. A collision requires that the
/// bodies were not overlapping in the last physics engine frame, but are overlapping now.
/// This class additionally resolves contacts, applying necessary impulses to the two bodies
/// to enact the results of the collision.
/// </summary>
class Contact {
	public PPhysicsBody _b1;
	public PPhysicsBody _b2;
    public Vector2 contactNormal; //_b2 to _b1
    public Vector2 rv;


    public void ResolveContact(float timeStep) {
        
        _b1.Revert();
        _b2.Revert();

        //Use Minkowski subtraction for axis aligned bounding box collision determination
        Vector2 p = _b2.position;


        Vector2 UR = new Vector2(_b1.UR.x + (_b2.UR.x - p.x), _b1.UR.y + (_b2.UR.y - p.y));
        Vector2 UL = new Vector2(_b1.LL.x - (_b2.UR.x - p.x), _b1.UR.y + (_b2.UR.y - p.y));
        Vector2 LL = new Vector2(_b1.LL.x - (_b2.UR.x - p.x), _b1.LL.y - (_b2.UR.y - p.y));
        Vector2 LR = new Vector2(_b1.UR.x + (_b2.UR.x - p.x), _b1.LL.y - (_b2.UR.y - p.y));

        //Retrieve intersection between minkowski subtraction and point segment intersection along contactNormal
        Vector2 top = Intersects(p, p + contactNormal, UL, UR);
        Vector2 right = Intersects(p, p + contactNormal, UR, LR);
        Vector2 left = Intersects(p, p + contactNormal, UL, LL);
        Vector2 bottom = Intersects(p, p + contactNormal, LL, LR);


        List<Vector2> intersections = new List<Vector2>();
        if (top != Vector2.positiveInfinity && top.x <= UR.x && top.x >= UL.x) intersections.Add(top);
        if (bottom != Vector2.positiveInfinity && bottom.x <= LR.x && bottom.x >= LL.x) intersections.Add(bottom);
        if (left != Vector2.positiveInfinity && left.y <= UL.y && left.y >= LL.y) intersections.Add(left);
        if (right != Vector2.positiveInfinity && right.y <= UR.y && right.y >= LR.y) intersections.Add(right);

        Vector2 min = Vector2.positiveInfinity;
        foreach (Vector2 v in intersections)
        {
            if (Vector2.Distance(min, p) > Vector2.Distance(v, p))
            {
                min = v;
            }
        }

        Vector2 hitNormal = Vector2.zero;

        if (min.y == LL.y || min.y == UR.y)
        {
            if (min == bottom)
            {
                hitNormal = new Vector2(0, -1);
            }
            else
            {
                hitNormal = new Vector2(0, 1);
            }
        }
        else if (min.x == LL.x || min.x == LR.x)
        {
            if (min == left)
            {
                hitNormal = new Vector2(-1, 0);
            }
            else
            {
                hitNormal = new Vector2(1, 0);
            }
        }

        float s = Vector2.Distance(p, min) / contactNormal.magnitude*0.9f; // 0 <= s <= 1 * timeStep

        float t = s * timeStep;
       
        _b1.Integrate(t);
        _b2.Integrate(t);

        
        float vc = Vector2.Dot(-rv, hitNormal);
        float coefficientOfRestitution = 0.8f;
        if (_b1.rebounds || _b2.rebounds)
        {
            coefficientOfRestitution = 0.1f;
        }
        float vcprime = coefficientOfRestitution * vc;
        float deltaV = vc - vcprime;
        float totalInverse = 1 / _b1.mass + 1 / _b2.mass;
        float g = deltaV * totalInverse;
        Vector2 impulseWithNormal = hitNormal * g;

        _b1.Velocity -= impulseWithNormal / _b1.mass;
        _b2.Velocity += impulseWithNormal / _b2.mass;


    }

    //checks if two segments intersect and returns point where they intersect.

    private Vector2 Intersects( Vector2 line1p1, Vector2 line1p2, Vector2 line2p1, Vector2 line2p2)
    {
        //line1
        float A1 = line1p2.y - line1p1.y;
        float B1 = line1p1.x - line1p2.x;
        float C1 = A1 * line1p1.x + B1 * line1p1.y;

        //line2
        float A2 = line2p2.y - line2p1.y;
        float B2 = line2p1.x - line2p2.x;
        float C2 = A2 * line2p2.x + B2 * line2p2.y;


        float delta = A1 * B2 - A2 * B1;
        if (Mathf.Abs(delta) <= Mathf.Epsilon)
        {
            return Vector2.positiveInfinity;
        }

        float x = (B2 * C1 - B1 * C2) / delta;
        float y = (A1*C2 - A2*C1) / delta;

        return new Vector2(x, y);
    }

	/// <summary>
	/// Records the colliding physics bodies and computes the contact normal
	/// </summary>
	/// <param name="b1">the first body</param>
	/// <param name="b2">the second body</param>
	public Contact(PPhysicsBody b1, PPhysicsBody b2) {
		_b1 = b1;
		_b2 = b2;
        contactNormal = (b2.oldPosition - b1.oldPosition);
        rv = b2.Velocity - b1.Velocity;
    }
}
