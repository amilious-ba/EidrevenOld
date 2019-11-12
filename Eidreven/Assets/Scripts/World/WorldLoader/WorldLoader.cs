using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoaderType{Standard}

public abstract class WorldLoader{

	private World world;
	private bool completedInitialBuild, completedInitialDraw;
	private List<WorldLoaderListener> listeners;

	//world loaders

	public WorldLoader(World world){
		this.world = world;
		this.listeners = new List<WorldLoaderListener>();
	}

	public abstract void initalizeAndDraw();
	public abstract void initialBuild();
	public abstract void initialDraw();
	public abstract Chunk getLoadedChunk(string name);
	public abstract Chunk foceGetChunk(string name);
	public abstract void update();

	public bool CompletedInitialDraw{get{return completedInitialDraw;}}
	public bool CompletedInitialBuild{get{return CompletedInitialBuild;}}

	public bool addListener(WorldLoaderListener listener){
		int count = listeners.Count;
		listeners.Add(listener);
		return (listeners.Count>count);
	}

	public bool removeListener(WorldLoaderListener listener){
		//int count = listeners.Count;
		return listeners.Remove(listener);
	}

	public void updateListeners(int value, int maxValue, string status){
		foreach(WorldLoaderListener listener in listeners){
			listener.updateStatus(value, maxValue, status);
		}
	}
}
