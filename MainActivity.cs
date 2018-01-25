using Android.App;
using Android.Widget;
using Android.OS;

namespace DesignLibrary
{
    [Activity(Label = "DesignLibrary", MainLauncher = true, Icon = "@drawable/icon", Theme = "MyTheme.Base")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
             SetContentView (Resource.Layout.Main);
        }
    }
}

