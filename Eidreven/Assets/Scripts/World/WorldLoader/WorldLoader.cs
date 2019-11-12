using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoaderType{Standard, Test}

public abstract class WorldLoader{

	
	public abstract void initalizeAndDraw();

	/**
	 * This method is used to get the chunk with the given index.
	 * If the chunk is not currently loaded it will return null,
	 * otherwise it will return the chunk.
	 * @param GamePoint index the index of the chunk you want to
	 * get if it is loaded.
	 * @returns Chunk contains the chunk if it is loaded, otherwise
	 * returns false.
	 */
	public abstract Chunk getLoadedChunk(GamePoint index);

	/**
	 * The method is used to get the chunk with the given index.
	 * If the chunk is not currently loaded it will load the chunk
	 * or create it before returning the chunk.
	 * @param GamePoint index the index of the chunk you want to
	 * get.
	 * @returns Chunk contains the loaded or newly created chunk.
	 */
	public abstract Chunk foceGetChunk(GamePoint index);

	/**
	 * This method is called by the world class on its update method.
	 */
	public abstract void update();




//////////////////////////////////////////////////////////////////////////
/// The code bellow is the abstract code for classes that extend this ///
/// this class.														  ///
/////////////////////////////////////////////////////////////////////////

	public bool CompletedInitialDraw{get{return completedInitialDraw;}}
	public bool CompletedInitialBuild{get{return completedInitialBuild;}}

	protected World world;
	protected bool completedInitialBuild, completedInitialDraw;
	private List<WorldLoaderListener> listeners;

	//world loaders

	public WorldLoader(World world){
		this.world = world;
		this.listeners = new List<WorldLoaderListener>();
	}

	public bool addListener(WorldLoaderListener listener){
		int count = listeners.Count;
		listeners.Add(listener);
		return (listeners.Count>count);
	}

	public bool removeListener(WorldLoaderListener listener){
		//int count = listeners.Count;
		return listeners.Remove(listener);
	}

	public void updateStatus(int value, int maxValue, string status){
		foreach(WorldLoaderListener listener in listeners){
			listener.updateStatus(value, maxValue, status);
		}
	}

	public void worldInitLoadComplete(){
		foreach(WorldLoaderListener listener in listeners)listener.worldInitLoadComplete();
	}

	public static WorldLoader CreateLoader(World world, LoaderType loaderType){
		WorldLoader loader = null;
		switch(loaderType){
			case LoaderType.Standard: loader = new StandardWorldLoader(world);break;
			case LoaderType.Test: loader = new TestWorldLoader(world);break;
			default: loader = new StandardWorldLoader(world);break;
		}
		loader.addListener(world);
		return loader;
	}
}
