using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Acx.Forms.Barcode.Droid;
using Acx.Forms.Barcode.Droid.Camera;
using Acx.Forms.Barcode.Droid.Logger;
using Acx.Forms.Barcode.Droid.View;
using Acx.Forms.Barcode.Pcl;
using Acx.Forms.Barcode.Pcl.Logger;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BarcodeScanner), typeof(BarcodeScannerRenderer))]
namespace Acx.Forms.Barcode.Droid
{
    public class BarcodeScannerRenderer : ViewRenderer<BarcodeScanner, SurfaceView>, ISurfaceHolderCallback
    {
        private static Configuration config;
        private readonly CameraConfigurator configurator;
        private CameraService cameraServiceHolder;

        private CameraService CameraService {
            get {
                if (null == cameraServiceHolder) {
                    var factory = new CameraServiceFactory ();
                    cameraServiceHolder = factory.Create (Context, Element, configurator);
                }

                return cameraServiceHolder;
            }
        }

        private Lazy<Platform> platform;

        private Platform Platform {
            get {
                return platform.Value;
            }
        }

        private bool HasValidSurface {
            get {
                return Control?.Holder.Surface.IsValid == true;
            }
        }

        public static void Init ()
        {
            Init (new Configuration ());
        }

        public static void Init (Configuration config)
        {
            BarcodeScannerRenderer.config = config;
        }

        public BarcodeScannerRenderer ()
        {
            configurator = new CameraConfigurator ().SetConfiguration (config);
            platform = new Lazy<Platform> (() => new Platform ());
        }

        public async void SurfaceCreated (ISurfaceHolder holder)
        {
            // Nothing to do.
        }

        public async void SurfaceChanged (ISurfaceHolder holder, global::Android.Graphics.Format format, int width, int height)
        {
            if (!HasValidSurface) {
                return;
            }

            // portrait mode
            if (height > width) {
                CameraService.SetViewSize (height, width);
            } else {
                CameraService.SetViewSize (width, height);
            }

            if (!Element.IsEnabled) {
                return;
            }

            CameraService.HaltPreview ();
            await Task.Run (() => {
                CameraService.StartPreview (holder);
            });
        }

        public void SurfaceDestroyed (ISurfaceHolder holder)
        {
            Element.IsEnabled = false;
        }

        protected override void OnElementChanged (Xamarin.Forms.Platform.Android.ElementChangedEventArgs<BarcodeScanner> e)
        {
            base.OnElementChanged (e);

            if (Element != null && Element.IsEnabled)
                InitializeView();
        }

        protected async override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged (sender, e);

            if (e.PropertyName == Xamarin.Forms.VisualElement.IsEnabledProperty.PropertyName) {
                if (Element.IsEnabled) {
                    InitializeView();
                }

                if (!Element.IsEnabled) {
                    ReleaseCamera ();
                }
            }

            if (e.PropertyName == BarcodeScanner.PreviewActiveProperty.PropertyName) {
                if (Element.PreviewActive) {
                    await Task.Run (() => CameraService.StartPreview (Control.Holder));
                }

                if (!Element.PreviewActive) {
                    CameraService.HaltPreview ();
                }
            }

            if (e.PropertyName == BarcodeScanner.TorchProperty.PropertyName) {

                if (!Platform.HasFlashPermission) {
                    return;
                }

                if (!Platform.HasFlash) {
                    return;
                }

                CameraService.ToggleTorch (Element.Torch);
            }

            if (e.PropertyName == BarcodeScanner.BarcodeDecoderProperty.PropertyName) {
                if (Element.BarcodeDecoder) {
                    CameraService.StartDecoder ();
                }

                if (!Element.BarcodeDecoder) {
                    CameraService.StopDecoder ();
                }
            }
        }

        private void InitializeView() 
        {
            if (!Platform.HasCameraPermission) {
                return;
            }

            if (!Platform.HasCamera) {
                return;
            }

            if (!Platform.IsGmsReady) {
                return;
            }

            if (Control != null) {
                ActivatePreview ();
                return;
            }

            var surfaceView = new SurfaceView (Context);
            surfaceView.Holder.AddCallback (this);
            SetNativeControl (surfaceView);

            Element.CameraOpened += async (sender, args) => {
                if (Element.PreviewActive) {
                    await Task.Run (() => {
                        if (Element.IsEnabled) {
                            CameraService.StartPreview (Control.Holder);
                            CameraService.ToggleTorch (Element.Torch);
                        }
                    });
                }
            };
        }

        private async void ActivatePreview() 
        {
            if (Element.PreviewActive) {
                await Task.Run (() => {
                    if (Element.IsEnabled) {
                        CameraService.StartPreview (Control.Holder);
                        CameraService.ToggleTorch (Element.Torch);
                    }
                });
            }
        }

        protected override void Dispose (bool disposing)
        {
            ReleaseCamera ();
            base.Dispose (disposing);
        }

        private void ReleaseCamera ()
        {
            CameraService?.ReleaseCamera ();
            cameraServiceHolder = null;
        }
    }
}
