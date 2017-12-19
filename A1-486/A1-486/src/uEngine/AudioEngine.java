package uEngine;

import javax.sound.sampled.*;
import java.util.*;

public class AudioEngine {
	// Clips have a name, associated with a file name where the clip's audio
	// is stored. A dictionary is used to store this association.
	private Dictionary<String,String> _clips = new Hashtable<String,String>(); 
	
	public void addClip(String clipName, String clipFileName) {
		_clips.put(clipName, clipFileName);
	}
	
	public void playOneShot(String clipName) {
		// Complete and uncomment the code below
		// String clipFileName = ...
		String clipFileName = _clips.get(clipName);
		play(clipFileName);
	}
	
    private synchronized void play(final String fileName) {
    	// This code adapted from http://noobtuts.com/java/play-sounds
        // Note: use .wav files             
        new Thread(new Runnable() { 
            public void run() {
                try {
                    Clip clip = AudioSystem.getClip();
                    AudioInputStream inputStream =
                    		AudioSystem.getAudioInputStream(new java.io.File(fileName));
                    clip.open(inputStream);
                    clip.start(); 
                } catch (Exception e) {
                    System.out.println("play sound error: " + e.getMessage() + " for " + fileName);
                }
            }
        }).start();
    }
}
