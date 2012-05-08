using System;
using System.Threading;
using Sifteo;


namespace net.addictivesoftware.sifteo.AnimalMatch {
	// ## Wrapper ##
  // "Wrapper" is not a specific API, but a pattern that is used in many Sifteo
  // apps. A wrapper is an object that bundles a Cube object with game-specific
  // data and behaviors.
  public class CubeWrapper {

    public AnimalMatchApp app;
    public Cube cube;
    public int index;

	private int shakeCount = 0;
	private int tiltCount = 0;
	public bool tiltedXChanged = false;
	public int tiltedX = 1;
		
    public bool needDraw = false;

    public CubeWrapper(AnimalMatchApp app, Cube cube) {
		this.app = app;
		this.cube = cube;
		cube.userData = this;
		index = 0;
			
		cube.ButtonEvent += OnButton;
		cube.TiltEvent += OnTilt;
    }

     private void OnButton(Cube cube, bool pressed) {
		Log.Debug("Pressed a button");
		if (pressed) {
			Log.Debug("playing sound for " + app.imageNames[app.wrappers[cube.UniqueId].index]);
			Sound sound = app.sounds[app.imageNames[app.wrappers[cube.UniqueId].index]];
				Log.Debug("playing...");
				sound.Play(1,0);	
		}
    }

    private void OnTilt(Cube cube, int tiltX, int tiltY, int tiltZ) {
		Log.Debug("Tilt: {0} {1} {2}", tiltX, tiltY, tiltZ);
		if (tiltedX != tiltX) {
			tiltedX = tiltX;
			tiltedXChanged = true;
		}
		if (tiltX == 1) {
			tiltedXChanged = false;
		}
    }

    private void OnShakeStarted(Cube cube) {
    }

    private void OnShakeStopped(Cube cube, int duration) {
    }

    private void OnFlip(Cube cube, bool newOrientationIsUp) {
    }

    public void DrawSlide() {

      String imageName = this.app.imageNames[this.index];

      int screenX = 0;
      int screenY = 0;

      int imageX = 0;
      int imageY = 0;

      int width = 128;
      int height = 128;

      int scale = 1;

      int rotation = 0;

      cube.FillScreen(Color.Black);
			
      cube.Image(imageName, screenX, screenY, imageX, imageY, width, height, scale, rotation);

      cube.Paint();
    }
		
	private void ShowNextImage() {
		index = index + 1;
		if (index >= app.imageLength) {
			index = 0;		
		}
		needDraw = true;
	}
	private void ShowPreviousImage() {
		index = index - 1;
		if (index < 0) {
			index = app.imageLength-1;		
		}
		needDraw = true;
	}
		
		
    // This method is called every frame by the Tick in SlideShowApp (see above.)
    public void Tick() {

		if (cube.IsShaking) {
			shakeCount++;
		}
		if (shakeCount > 5) {
			shakeCount = 0;
			index = app.random.Next(app.imageLength);
			needDraw = true;
		}
			
		if (tiltedXChanged) {
			tiltCount++;		
		}
		if (tiltCount > 5 && tiltedX == 2) {
			ShowNextImage();
			tiltCount = 0;
		}

		if (tiltCount > 5 && tiltedX == 0) {
			ShowPreviousImage();
			tiltCount = 0;
		}
			
		if (needDraw) {
			needDraw = false;
			DrawSlide();
		}
    }

  }
}