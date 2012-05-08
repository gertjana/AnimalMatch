using System;
using System.Threading;
using Sifteo;


namespace net.addictivesoftware.sifteo.AnimalMatch {

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
		if (pressed) {
			Sound sound = app.sounds[app.imageNames[app.wrappers[cube.UniqueId].index]];
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

    public void DrawSlide() {
      String imageName = this.app.imageNames[this.index];

      cube.FillScreen(Color.Black);
      cube.Image(imageName, 0, 0, 0, 0, 128, 128, 1, 0);
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