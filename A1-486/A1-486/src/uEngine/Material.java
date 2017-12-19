package uEngine;

import java.awt.*;

public class Material extends Component {
	public Color color;
	public boolean isVisible;
	
	public Material(Color _color) {
		// ...
		color = _color;
		isVisible = true;
	}
	
	public Material() {
		this(Color.BLACK);
	}
}
