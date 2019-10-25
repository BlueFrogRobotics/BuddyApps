

namespace BuddyApp.FreezeDance
{
    //    public class FreezeDanceLayout : AWindowLayout
    //    {
    //		private Gauge mGaugeValOne;

    //        public override void Build()
    //        {		
    //            /*
    //             * Create needed widgets
    //             * ==> Which widget do I need for my app settings ?
    //             */
    //            mGaugeValOne = CreateWidget<Gauge>();

    //            /*
    //             * Set widgets parameters
    //             */
    //            mGaugeValOne.Slider.minValue = 0;
    //            mGaugeValOne.Slider.maxValue = 10;
    //            mGaugeValOne.Slider.wholeNumbers = true;
    //            mGaugeValOne.DisplayPercentage = true; /* Only the display will be in percentage, the value will still be within 0 and 10 */

    //            /*
    //             * Retrieve app data and display them inside the view
    //             * ==> What info must be displayed ?
    //             */ 
    //            mGaugeValOne.Slider.value = FreezeDanceData.Instance.MyValue;

    //            /*
    //            * Set command to widgets
    //            * At each interaction with a widget, a callback will be called
    //            * ==> What must happen when I interacted with a widget ?
    //            */
    //            mGaugeValOne.OnUpdateEvent((iVal) => {
    //				FreezeDanceData.Instance.MyValue = iVal;
    //			});
    //		}

    //        public override void LabelizeWidgets()
    //        {
    //            mGaugeValOne.Label = "AN INTEGER";
    //        }
    //    }
}