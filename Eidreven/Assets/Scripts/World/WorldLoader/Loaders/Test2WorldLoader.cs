using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2WorldLoader : WorldLoader{

	//load and draw initial chunks
	
	//on updated lastPos
		//build the new chunks
		//add the new chunks to a draw queue
		//remove old chunks
	//if draw queue is not empty
		//deque based on direction	

	//private variables
	private ConcurrentDictionary<GamePoint, Chunk> loadedChunks;
	private GamePoint lastbuildPos;
	private LockFreeQueue<GamePoint> unloadChunks;
	private int steps=0, progress=0;
	private int counter = 0;
    
	public Test2WorldLoader(World world):base(world){		
		loadedChunks = new ConcurrentDictionary<GamePoint, Chunk>();
		unloadChunks = new LockFreeQueue<GamePoint>();
	}

	public override void initalizeAndDraw(){
		//set up
		GamePoint playerPos = GamePoint.roundVector3(world.Player.transform.position);
		playerPos.y = WorldGenerator.GenerateHeight(playerPos.x, playerPos.z) + 1;
		world.Player.transform.position = playerPos;
		lastbuildPos = Chunk.IndexAtPosition(playerPos);
		BuildChunk(lastbuildPos);
		//calculate build steps
		steps = calculateSteps(lastbuildPos.y,Global.LoadRadius);
		//start generating world
		world.Exicute(BuildWorld(lastbuildPos,getLookingDirection(),Global.LoadRadius));
	}

	public override Chunk getLoadedChunk(GamePoint index){
		Chunk chunk = null;
		loadedChunks.TryGetValue(index, out chunk);
		return chunk;
	}

	public override Chunk foceGetChunk(GamePoint index){
		//later this metheod will c
		return getLoadedChunk(index);
	}

	//this method will be called on the world update method
	public override void update(){
		counter++;if(counter<100){return;}counter = 0;
		if(!completedInitialBuild||!completedInitialDraw){
			//the inital load is still running
			if(!completedInitialBuild)return;
			world.Exicute(DrawChunks());
		}else{
			//run the world management script
			GamePoint playerPos = GamePoint.roundVector3(world.Player.transform.position);
			GamePoint movement = lastbuildPos - Chunk.IndexAtPosition(playerPos);
			if(movement.magnitude >= Global.BuildWhenMovedXChunks){
				lastbuildPos = Chunk.IndexAtPosition(playerPos);
				world.StopCoroutine("BuildWorld");
				world.Exicute(BuildWorld(lastbuildPos,getLookingDirection(),Global.LoadRadius));
			}
			//draw chunks
			world.Exicute(DrawChunks());
			//remove chunks
			world.Exicute(RemoveOldChunks());
		}
	}

	private Direction getLookingDirection(){
		int looking = (int)world.Player.transform.rotation.eulerAngles.y;
		looking = (looking+45); if(looking>360)looking-=360;looking/=90;
		switch(looking){
			case 0: return Direction.NORTH;
			case 1: return Direction.EAST;
			case 2: return Direction.SOUTH;
			case 3: return Direction.WEST;
			default: return Direction.NORTH;
		}
	}




	private bool BuildChunk(GamePoint ChunkIndex){
		if(ChunkIndex.y<0||ChunkIndex.y>Global.ChunksTall)return false;
		Chunk chunk = null;
		//if chunk does not exist create it
		if(!loadedChunks.TryGetValue(ChunkIndex,out chunk)){
			chunk = new Chunk(ChunkIndex, world);
			//try to add chunk
			if(loadedChunks.TryAdd(ChunkIndex,chunk)){
				chunk.Build(); return true;
			}else chunk.Destroy();
		}return false; //the chunk exists
	}

	IEnumerator BuildWorld(GamePoint index, Direction facing, int radius){
		if(radius<=0){yield break;}radius--;
		//first build in the passed direction
		GamePoint buildPos = index.moveDirection(facing,1);
		if(BuildChunk(buildPos)){updatefirstLoad("building "+buildPos);}
		world.Exicute(BuildWorld(buildPos, facing, radius));
		yield return null;
		//build down
		GamePoint down = index.moveDirection(Direction.DOWN,1);
		if(BuildChunk(down)){updatefirstLoad("building "+down);}
		world.Exicute(BuildWorld(down, facing, radius));
		yield return null;
		
		//then build to the left
		GamePoint left = index.moveDirection(Directions.rotateNWSE(facing),1);
		if(BuildChunk(left)){updatefirstLoad("building "+left);}
		world.Exicute(BuildWorld(left, facing, radius));
		yield return null;

		//then build to the right
		GamePoint right = index.moveDirection(Directions.rotateNESW(facing),1);
		if(BuildChunk(right)){updatefirstLoad("building "+right);}
		world.Exicute(BuildWorld(right, facing, radius));
		yield return null;

		//build up
		GamePoint up = index.moveDirection(Direction.UP,1);
		if(BuildChunk(up)){updatefirstLoad("building "+up);}
		world.Exicute(BuildWorld(up, facing, radius));
		yield return null;

		//then build behind
		GamePoint invert = index.moveDirection(Directions.invertDirection(facing),1);
		if(BuildChunk(invert)){updatefirstLoad("building "+invert);}
		world.Exicute(BuildWorld(invert, facing, radius));
		yield return null;
	}

	public bool updatefirstLoad(string status){
		if(completedInitialBuild&&completedInitialDraw)return false;
		progress++;
		if(steps==progress&&!completedInitialBuild){
			completedInitialBuild = true;
			progress = 0; steps -=(steps/15);
		}else if(steps==progress){
			completedInitialDraw = true;
			this.worldInitLoadComplete();
			return true;
		}
		this.updateStatus(progress, steps, status);
		return true;
	}

	public int calculateSteps(int y, int radius){
		int chunkCount = calculateLayerSteps(radius);
		//calculate up
		int up = y+radius; int upRad = radius-1;
		if(up>Global.ChunksTall)up=Global.ChunksTall;
		for(int i=y+1;i<up;i++)
			chunkCount += calculateLayerSteps(upRad--);
		//calculate down
		int down = y-radius;  int downRad = radius-1;
		if(down<0)down=0;
		for(int i=y-1;i>=down;i--)
			chunkCount+= calculateLayerSteps(downRad--);
		return chunkCount;
	}

	public int calculateLayerSteps(int radius){
		int calc = 0;
		for(int i=(radius*2)-1;i>0;i-=2)calc+=i;
		calc*=2;calc+=((radius*2)+1);
		return calc;
	}

	IEnumerator DrawChunks(){
		foreach(KeyValuePair<GamePoint, Chunk> value in loadedChunks){
			Chunk chunk = value.Value;
			if(Vector3.Distance(world.Player.transform.position,
				chunk.Position)>Global.LoadRadius*Global.ChunkSize){
				unloadChunks.Enqueue(value.Key);
			} else if(chunk.Status == ChunkStatus.DRAW){
				chunk.Draw();
				updatefirstLoad("drawing "+(chunk.Index));
			} yield return null;
		}
	}

	

	IEnumerator RemoveOldChunks(){
		GamePoint chunkIndex = new GamePoint(0,0,0);
		while(unloadChunks.Dequeue(out chunkIndex)){
			world.Exicute(RemoveChunk(chunkIndex));
		}yield return null;
	}

	IEnumerator RemoveChunk(GamePoint index){
		Chunk chunk;
		if(loadedChunks.TryGetValue(index,out chunk)){
			chunk.Destroy();
			loadedChunks.TryRemove(index, out chunk);
			yield return null;
		}
	}


}
