using System;
using System.Collections;
using System.Collections.Generic;
using Sifteo;

namespace net.addictivesoftware.sifteo.AnimalMatch {

  public class AnimalMatchApp : BaseApp {

    public String[] imageNames;
	public ImageInfo goodImage;
	public ImageInfo badImage;
		
	public Dictionary<String, Sound> sounds = new Dictionary<String, Sound>();	
		
	public int imageLength;
    public Dictionary<String, CubeWrapper> wrappers = new Dictionary<String, CubeWrapper>();
    public Random random = new Random();		
		
    public override void Setup() {

      imageNames = LoadImageAndSoundIndex();

      foreach (Cube cube in CubeSet) {
        CubeWrapper wrapper = new CubeWrapper(this, cube);
		wrapper.index = random.Next(imageLength);
        wrappers.Add(cube.UniqueId, wrapper);
        wrapper.DrawSlide();
      }

      // ## Event Handlers ##
      // Objects in the Sifteo API (particularly BaseApp, CubeSet, and Cube)
      // fire events to notify an app of various happenings, including actions
      // that the player performs on the cubes.
      //
      // To listen for an event, just add the handler method to the event. The
      // handler method must have the correct signature to be added. Refer to
      // the API documentation or look at the examples below to get a sense of
      // the correct signatures for various events.
      //
      // **NeighborAddEvent** and **NeighborRemoveEvent** are triggered when
      // the player puts two cubes together or separates two neighbored cubes.
      // These events are fired by CubeSet instead of Cube because they involve
      // interaction between two Cube objects. (There are Cube-level neighbor
      // events as well, which comes in handy in certain situations, but most
      // of the time you will find the CubeSet-level events to be more useful.)
      CubeSet.NeighborAddEvent += OnNeighborAdd;
      CubeSet.NeighborRemoveEvent += OnNeighborRemove;
    }

    // ## Neighbor Add ##
    // This method is a handler for the NeighborAdd event. It is triggered when
    // two cubes are placed side by side.
    //
    // Cube1 and cube2 are the two cubes that are involved in this neighboring.
    // The two cube arguments can be in any order; if your logic depends on
    // cubes being in specific positions or roles, you need to add logic to
    // this handler to sort the two cubes out.
    //
    // Side1 and side2 are the sides that the cubes neighbored on.
    private void OnNeighborAdd(Cube cube1, Cube.Side side1, Cube cube2, Cube.Side side2)  {
      	Log.Debug("Neighbor add: {0}.{1} <-> {2}.{3}", cube1.UniqueId, side1, cube2.UniqueId, side2);
		if (wrappers[cube1.UniqueId].index == wrappers[cube2.UniqueId].index) {	
			ShowGoodImage(cube1);	
			ShowGoodImage(cube2);
			sounds[imageNames[wrappers[cube1.UniqueId].index]].Play (1,0);
		} else {
			ShowBadImage(cube1);		
			ShowBadImage(cube2);		
		}
    }

    // ## Neighbor Remove ##
    // This method is a handler for the NeighborRemove event. It is triggered
    // when two cubes that were neighbored are separated.
    //
    // The side arguments for this event are the sides that the cubes
    // _were_ neighbored on before they were separated. If you check the
    // current state of their neighbors on those sides, they should of course
    // be NONE.
    private void OnNeighborRemove(Cube cube1, Cube.Side side1, Cube cube2, Cube.Side side2)  {
		//wrappers[cube1.UniqueId].index = random.Next(imageLength);
		//wrappers[cube2.UniqueId].index = random.Next(imageLength);
			
		wrappers[cube1.UniqueId].DrawSlide();
		wrappers[cube2.UniqueId].DrawSlide();
    }
		
		
    public override void Tick() {
		foreach (KeyValuePair<String, CubeWrapper> wrapper in wrappers) {
			wrapper.Value.Tick();
		}
    }

    private String[] LoadImageAndSoundIndex() {
      ImageSet imageSet = this.Images;
      ArrayList nameList = new ArrayList();
      foreach (ImageInfo image in imageSet) {
		
		if (image.name == "bad") {
			badImage = image;			
		} else if (image.name == "good") {
			goodImage = image;
		} else {
			nameList.Add(image.name);		
			sounds.Add(image.name, Sounds.CreateSound(image.name));
		}
      }
      String[] rv = new String[nameList.Count];
      for (int i=0; i<nameList.Count; i++) {
        rv[i] = (string)nameList[i];
      }
	  imageLength = nameList.Count;	
      return rv;
    }

	public void ShowImage(Cube cube, String name) {
		cube.FillScreen(Color.Black);		
		cube.Image(name, 0, 0, 0, 0, 128, 128, 1, 0);
		cube.Paint();		
	}
		
	public void ShowBadImage(Cube cube) {
		ShowImage(cube, "bad");
	}
	public void ShowGoodImage(Cube cube) {
		ShowImage(cube, "good");
	}
}