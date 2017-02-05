using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Leap;

namespace Test_Leap_Motion_C
{
    public class LeapListener : Listener
    {


        private Controller current;
        private Form1 f1;


        public LeapListener(Controller parent, Form1 f)
        {
            current = parent;
            f1 = f;

        }
        private int backcount = 0, entercount = 0;


        public virtual void onFrame()
        {

            Frame curr = current.Frame();
            bool backgesture = false, entergesture = false, fastback = false;
            foreach (Gesture a in curr.Gestures())
            {
                if (a.Type == Gesture.GestureType.TYPE_CIRCLE)
                {
                    CircleGesture circle = new CircleGesture(a);
                    if (circle.Radius < 20 && circle.Radius > 7 && backcount > 8)
                    {
                        fastback = true;
                    }
                    if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Math.PI / 4)
                    {
                        entergesture = true;

                    }
                    else
                    {
                        backgesture = true;
                    }
                }
            }
            if (entergesture)
            {
                entercount++;
                if (entercount > 20)
                {
                    entercount = 0;
                    f1.enter();

                }
                backcount = 0;
            }
            else
            {
                entercount = 0;
            }
            if (backgesture)
            {
               
                entercount = 0;
                backcount++;
                if (fastback)
                {
                    backcount += 3;
                }
                if (backcount > 35)
                {
                    backcount = 0;


                }
            }
            else
            {
                backcount = 0;
            }


        }


        

        public void run()
        {

            while (true)
            {
                try
                {
                    Thread.Sleep(30);
                }
                catch (ThreadInterruptedException e)
                {
                   // Console.WriteLine(e.ToString());
                   // Console.Write(e.StackTrace);
                }
                onFrame();
            }
        }



        /*public  void onInit(Controller controller){
        System.out.println("Initialized");
        }*/

        public virtual void onConnect(Controller controller)
        {
            Console.WriteLine("");
            Console.WriteLine("------------| Leap Motion تم الاتصال بجهاز الـ |------------");

        }


        public virtual void onDisconect(Controller controller)
        {
            Console.WriteLine("------------| Leap Motion تم فصل الاتصال بجهاز الـ |------------");
        }


    }
}