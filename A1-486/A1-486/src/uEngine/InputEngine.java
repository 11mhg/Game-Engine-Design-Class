package uEngine;

import java.awt.MouseInfo;
import java.awt.event.*;

import javax.swing.*;

import java.util.*;

public class InputEngine {

	private JFrame _inputSource;
	private Dictionary<Integer,KeyChange> _keys = new Hashtable<Integer, KeyChange>();
	private ArrayList<Integer> mouse = new ArrayList<Integer>();
	private Boolean leftMouse;
	private Boolean rightMouse;
	private static InputEngine _input = null;
	public static final int LEFTMOUSE = 0;
	public static final int RIGHTMOUSE = 1;
	
	private KeyEventProcessor _keyEventProcessor;
	private MouseEventProcessor _mouseEventProcessor;
	
	/**
	 * Provide access to the Input component.
	 * @return the Input component
	 */
	public static InputEngine getInstance() {
		if(_input == null) {
			_input = new InputEngine();
		}
		return _input;
	}
	
	/**
	 * Specifies the time that the current frame started.
	 * @param time
	 */
	public void startFrame() {
		leftMouse = false;
		rightMouse = false;
		// Flag all existing statuses in dictionary as no longer referring
		// to current frame.
		for(Enumeration<Integer> ke = _keys.keys(); ke.hasMoreElements();) {
			Integer keyCode = ke.nextElement();
			KeyChange kc = _keys.get(keyCode);
			kc.currentFrame = false;
			_keys.put(keyCode, kc);
		}
		
		// Copy all buffered events to the dictionary
		for(KeyEvent e : _keyEventProcessor.getEvents()) {
			// Special case - Java auto-repeats events when key is held down. We
			// only want to record the first one. If this is a key pressed event and
			// there is already a key pressed event in the dictionary for this key,
			// throw the event away.
			if(e.getID() == KeyEvent.KEY_PRESSED) {
				KeyChange kc = _keys.get(e.getKeyCode());
				if(kc != null && kc.state == KeyChange.KEY_DOWN) {
					// this is a repeat - throw away this event
					continue;
				}
			}
			
			// Create a KeyChange object recording the event and add it to the dictionary
			KeyChange kc = new KeyChange();
			kc.currentFrame = true;
			if(e.getID() == KeyEvent.KEY_PRESSED) {
				kc.state = KeyChange.KEY_DOWN;
			} else {
				assert e.getID() == KeyEvent.KEY_RELEASED;
				kc.state = KeyChange.KEY_UP;
			}
			_keys.put(e.getKeyCode(), kc);
		}
		
		Boolean temp[] = _mouseEventProcessor.getMouseEvents();
		leftMouse = temp[0];
		rightMouse = temp[1];
	}
	
	/**
	 * Returns true if this key is currently depressed
	 * @param key
	 * @return
	 */
	public boolean getKey(int key) {
		// System.out.println("Checking whether key is down: " + key);
		KeyChange kc = _keys.get(key);
		if(kc == null) {
			// no events have been received for this key - assume not pressed
			return false;
		}
		
		if(kc.state == KeyChange.KEY_DOWN) {
			return true;
		}
		
		return false;
	}

	/**
	 * Returns true if this key has been depressed in the last frame
	 * @param key
	 * @return
	 */
	public boolean getKeyDown(int key) {
		// System.out.println("Checking whether key is pressed this frame: " + key);
		KeyChange kc = _keys.get(key);
		if(kc == null) {
			// no events have been received for this key - assume not pressed
			//System.out.println("getKeyDown: no record for: " + key);
			return false;
		}
		
		if(kc.state == KeyChange.KEY_DOWN && kc.currentFrame) {
			//System.out.println("getKeyDown: depressed in last frame: " + key);
			return true;
		}
		
		//System.out.println("getKeyDown: key depressed, but not in last frame: " + key);
		return false;
	}
	
	/**
	 * Returns true if this key has been released in the last frame
	 * @param key
	 * @return
	 */
	public boolean getKeyUp(int key) {
		// System.out.println("Checking whether key is released this frame: " + key);
		KeyChange kc = _keys.get(key);
		if(kc == null) {
			// no events have been received for this key - assume not pressed
			return false;
		}
		
		if(kc.state == KeyChange.KEY_UP && kc.currentFrame) {
			return true;
		}
		
		return false;
	}
	
	// Adding mouse input is not required for this assignment, but is an optional
	// challenge for no extra credit
	public boolean getMouseButton(int leftOrRight) {
		if (leftOrRight == LEFTMOUSE) {
			return leftMouse;
		}
		return rightMouse;
	}

	public boolean getMouseButtonDown(int leftOrRight) {
		if (leftOrRight == LEFTMOUSE) {
			return leftMouse;
		}
		return rightMouse;
	}

	public boolean getMouseButtonUp(int leftOrRight) {
		if (leftOrRight == LEFTMOUSE) {
			return leftMouse;
		}
		return rightMouse;
	}

	public Vector2 getMouse() {
		float m_X = (float)MouseInfo.getPointerInfo().getLocation().getX();
		float m_Y = (float)MouseInfo.getPointerInfo().getLocation().getY();
		return new Vector2(m_X,m_Y);
	}
	
	/**
	 * Sets the input source for the InputEngine to the JFrame context.
	 * @param jFrame context for the InputEngine to poll
	 */
	public void setInputSource(JFrame jFrame) {
		_inputSource = jFrame;
		
		// Ensure that all inputs are captured by this component, including "traversal"
		// inputs such as the tab key.
		_inputSource.setFocusTraversalKeysEnabled(false);
		
		// Add the listener for key events
		_keyEventProcessor = new KeyEventProcessor();
		_mouseEventProcessor = new MouseEventProcessor();
		_inputSource.addKeyListener(_keyEventProcessor);
		_inputSource.addMouseListener(_mouseEventProcessor);
	}
	
	/**
	 * Private constructor used to support singleton design pattern.
	 */
	private InputEngine() {}
	
	class KeyChange {
		public static final int KEY_UP = 0;
		public static final int KEY_DOWN = 1;
		
		public int state;	// KEY_UP or KEY_DOWN
		public boolean currentFrame;	// true if this change was in the current frame
	}
	
	/**
	 * Class to deal with key events. Events are buffered, and returned in one chunk
	 * when requested by the getEvents method.
	 * @author Nicholas Graham
	 *
	 */
	class KeyEventProcessor implements KeyListener {
		private List<KeyEvent> _events = new ArrayList<KeyEvent>();
		
		public synchronized void keyPressed(KeyEvent e) {
			_events.add(e);
			//System.out.println("Got key pressed event " + e.getKeyCode());
		}
		
		public synchronized void keyReleased(KeyEvent e) {
			_events.add(e);
			//System.out.println("Got key released event");
		}
		
		public synchronized void keyTyped(KeyEvent e) {
			//System.out.println("Got key typed event");
		}
		
		/**
		 * Returns all new keyboard events since the last time this method
		 * was called.
		 * @return list of new keyboard events
		 */
		public synchronized List<KeyEvent> getEvents(){
			List<KeyEvent> theEvents = _events;
			_events = new ArrayList<KeyEvent>();
			
			if(theEvents.size() > 0) {
				//System.out.println("** Returning events: " + theEvents.size());
			}
			return theEvents;
		}
	}
	
	class MouseEventProcessor implements MouseListener {
		Boolean RM = false;
		Boolean LM = false;
		
		public Boolean[] getMouseEvents() {
			Boolean[] toRet = new Boolean[2];
			toRet[0] = LM;
			toRet[1] = RM;
			LM = false;
			RM = false;
			return toRet;
		}
		
		@Override
		public void mouseClicked(MouseEvent m) {
			if (SwingUtilities.isRightMouseButton(m)) {
				RM = true;
			}else if (SwingUtilities.isLeftMouseButton(m)) {
				LM = true;
			}
		}

		@Override
		public void mouseEntered(MouseEvent m) {
			//Forget This.			
		}

		@Override
		public void mouseExited(MouseEvent m) {
			//Forget this.
		}

		@Override
		public void mousePressed(MouseEvent m) {
			if (SwingUtilities.isRightMouseButton(m)) {
				RM = true;
			}else if (SwingUtilities.isLeftMouseButton(m)) {
				LM = true;
			}
		}

		@Override
		public void mouseReleased(MouseEvent m) {
			if (SwingUtilities.isRightMouseButton(m)) {
				RM = false;
			}else if (SwingUtilities.isLeftMouseButton(m)) {
				LM = false;
			}
		}
		
	}
	
	
}