# Acx.Forms.Barcode

## What is this?

Acx.Forms.Barcode is a **Xamarin.Forms view for scanning barcodes** that are based on [based on RB.Forms.Barcode](https://github.com/rebuy-de/rb-forms-barcode). 
It provides continuous scanning and aims to give high control to the user along with high stability.

[Available via Nuget](https://www.nuget.org/packages/Acx.Forms.Barcode), full of awesomeness and  unicorns.

**Please note** that the library currently supports Android and iOS.

We are very eager about your feedback, so do not hesitate to create an issue or feel free to improve our code via a contribution.

### Features

* Fully Xamarin.Forms compatible. Add elements on top and adapt the UI to your needs.
* Lots of configuration options, bindable properties and events: Torch control, rotation support, preview freezing and other fine grained controls.
* Build for continuous scanning!
* Utilizing [Google Play Services Vision API](https://developers.google.com/vision/) on Android for best possible barcode scanning performance.
* iOS scanner uses the AVFoundation.

## Setup

1. Install the [package via nuget](https://www.nuget.org/packages/Acx.Forms.Barcode) into your PCL and platform specific projects.
2. [Set the appropriate Android permissions](http://developer.android.com/guide/topics/media/camera.html#manifest) to allow your app to access the camera and flash if need be.
4. Add the registration call `BarcodeScannerRenderer.Init();` to your platform specific Main class.
4. Use the `BarcodeScanner` class in your c# or xaml code.

Example of Android implementation of the Init call:


```
// MainActivity
protected override void OnCreate(Bundle bundle)
{
    base.OnCreate(bundle);
    Forms.Init(this, bundle);

    var config = new Configuration {
        Zoom = 5
    };

    BarcodeScannerRenderer.Init(config);

    LoadApplication(new App());
}
```

Example of iOS implementation of the Init call:


```
// AppDelegate
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    Forms.Init();
    BarcodeScannerRenderer.Init(
        new Configuration { 
            Barcodes = Barcode.BarcodeFormat.Ean13 | Barcode.BarcodeFormat.Ean8
        }
    );

    LoadApplication(new App());

    return base.FinishedLaunching(app, options);
}
```

## Usage

1. Create an instance of the `BarcodeScanner` class. Don't forget to give it a height and width.
2. Register an EventHandler for the `BarcodeScanner.BarcodeChanged` event to receive the detected barcodes.

For a hands-on experience, it is recommended to [take a look at the sample application](#sample).

### Please note

**Since beta 0.5.0**, the library only handles the most basic camera controls. The scanning starts as soon as the element is visible on the screen and stops when the view element gets removed from the stack.

Given the complexity of apps, there are a lot of combinations that prevent a reasonable automatic control of the camera. For example, when [sleeping](Sample/Sample.Pcl/Pages/ScannerPage.xaml.cs#L18)/[resuming](Sample/Sample.Pcl/Pages/ScannerPage.xaml.cs#L19) the device, when the [page gets disposed](Sample/Sample.Pcl/Pages/ScannerPage.xaml.cs#L74-L76) without notifying the view or another page get pushed onto the stack.

That's why you should weave in camera control code into the logic of your app by utilizing the offered bindings. Not doing so might lead to bad performance or unexpected camera exceptions.

Android Do's:

* Disable the preview when you add a page to the navigation stack.
* Disable the camera when the page gets removed from the stack.
* Disable the camera when sleeping the device.
* Ensure that only one instance currently is active.

### Bindable properties and events

All events are also available as `Command`s, the appropriate fields are suffixed accordingly. E.g. the command for `BarcodeChanged` event would be `BarcodeChangedCommand`.

What | Type | Description
---- | ---- | -----------
`BarcodeScanner.BarcodeChanged` | EventHandler | Raised only when the barcode text changes.
`BarcodeScanner.BarcodeDecoded` | EventHandler | Raised every time when a barcode is decoded from the preview, even if the value is the same as the preview one.
`BarcodeScanner.PreviewActivated` | EventHandler | Raised after the preview image gets active.
`BarcodeScanner.PreviewDeactivated` | EventHandler | Raised after the preview image gets deactivated.
`BarcodeScanner.CameraOpened` | EventHandler | Raised after the camera was obtained.
`BarcodeScanner.CameraReleased` | EventHandler | Raised after the camera was released.
`BarcodeScanner.Barcode` | Property | Holds the value of the last found barcode.
`BarcodeScanner.IsEnabled` | Property | If `true` opens the camera and activates the preview. `false` deactivates the preview and releases the camera.
`BarcodeScanner.PreviewActive` | Property | If `true` the preview image gets updated. `false` means no preview for you!
`BarcodeScanner.BarcodeDecoder` | Property | If `true` the decoder is active and tries to decode barcodes out of the image. `false` turns the decoder off, the preview is still active but barcodes will not be decoded.
`BarcodeScanner.Torch` | Property | Controls the camera flashlight if available and accessible. `true` sets the camera to torch mode (always on), `false` turns the flashlight off.

### Configuration

Configuration can be applied by passing a `Configuration` object to the `BarcodeScannerRenderer.Init()` method. As the available options are platform specific, the configuration has to be done in the according platform solution. The corresponding [Android](Acx.Forms.Barcode.Droid/Configuration.cs) class documentation should give you a solid understanding of the available options.

The compatibility mode is enabled to ensure the highest device compatibility by default.

Simple example:

    var config = new Configuration {
        // Some devices, mostly samsung, stop auto focusing as soon as one of the advanced features is enabled.
        CompatibilityMode = Build.Manufacturer.Contains("samsung")
    };

    BarcodeScannerRenderer.Init(config);

### Debugging

Acx.Forms.Barcode provides you with a tremendous amount of debug information, so check your application logs if anything goes wrong:

```
[Acx.Forms.Barcode] [BarcodeScannerRenderer] OnElementChanged
[Acx.Forms.Barcode] [BarcodeScannerRenderer] OnElementPropertyChanged
[Acx.Forms.Barcode] [BarcodeScannerRenderer] SurfaceCreated
[Acx.Forms.Barcode] [BarcodeScannerRenderer] SurfaceChanged
[Acx.Forms.Barcode] [CameraConfigurator] Focus Mode [continuous-picture]
[Acx.Forms.Barcode] [CameraConfigurator] Scene Mode [auto]
[Acx.Forms.Barcode] [CameraConfigurator] Metering area [True]
[Acx.Forms.Barcode] [CameraConfigurator] Focusing area [True]
[Acx.Forms.Barcode] [CameraConfigurator] Video stabilization [True]
[Acx.Forms.Barcode] [CameraConfigurator] White balance [auto]
[Acx.Forms.Barcode] [BarcodeScannerRenderer] OnElementPropertyChanged
[ScannerView] OnBarcodeChanged [886970911399 - UpcA]
[ScannerView] OnBarcodeDecoded [886970911399 - UpcA]
Decoded barcode [886970911399 - UpcA]
[Acx.Forms.Barcode] [BarcodeScannerRenderer] SurfaceDestroyed
[Acx.Forms.Barcode] [BarcodeScannerRenderer] OnElementPropertyChanged
[Acx.Forms.Barcode] [BarcodeScannerRenderer] Enabled [False]
[Acx.Forms.Barcode] [BarcodeScannerRenderer] OnElementPropertyChanged
[Acx.Forms.Barcode] [BarcodeScannerRenderer] Disposing
```

### FAKE options / Tasks

Execute `bin/fake <taskname>` to run a task or `bin/fake --<optionname>` for fake CLI options. First run `bin/fake install`.

(We are currently only supporting OSX to build the solution with FAKE. Feel free to add support for other platforms)

Available tasks:

```
* Restore
  Clean solution and afterwards restore all packages

* Gradlew
  Build Camera Source with Gradle

* Build
  Build all projects of the solution

```

