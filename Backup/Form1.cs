using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Leap;
using System.IO;

namespace Test_Leap_Motion_C
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            //var encoding = Encoding.Unicode;
            bool SHOW_SERIALIZED_GUESTURE = false;

            /*FileWriter file = new FileWriter("Result.txt");
            FileWriter file2 = new FileWriter("Result-Roll.txt");

            BufferedWriter bf = new BufferedWriter(file);
            BufferedWriter bf2 = new BufferedWriter(file2);*/

            //Console.OutputEncoding = encoding;

            //Console.WriteLine("-----------------------------------------------------------");
            //Console.WriteLine(" بدأ تشغيل البرنامج");
            //Console.WriteLine("-----------------------------------------------------------");

            //

            // Dictionary<string, List<double>> perfectLetters = new Dictionary<string, List<double>>();


            //  var perfectLetters = new Dictionary<string, string>();

            Dictionary<string, string[]> perfectLetters = new Dictionary<string, string[]>();




            //Console.WriteLine(" :ابجدية لغة الاشارة العربية المدعومه ");
            //Console.WriteLine("-----------------------------------------------------------");

            using (var file = File.OpenText("hands.txt"))
            {
                while (!file.EndOfStream)
                {
                    //Console.OutputEncoding = encoding;
                    var lineParts = file.ReadLine().Split(" ".ToCharArray(), 2); //split line around space characters
                    perfectLetters[lineParts[0]] = lineParts[1].Trim().Trim("[]".ToCharArray()).Split(',');




                    //  double d = Convert.ToDouble(perfectLetters[lineParts[0]]);

                    //Console.OutputEncoding = encoding;
                    //Console.Write(perfectLetters.Keys.Last().Trim('#', '@') + " ");


                }
                //Console.WriteLine();


            }

            //  Dictionary<String, Double> output = perfectLetters.ToDictionary(item => item.Key, item =>(Double)item.Value);

            var result = perfectLetters.ToDictionary(
         x => x.Key,
         x => x.Value.Select(y => Convert.ToDouble(y)).ToArray());
            

            Controller controller = new Controller();
            controller.EnableGesture(Gesture.GestureType.TYPE_CIRCLE);

            LeapListener listener = new LeapListener(controller, this);


            controller.AddListener(listener);

            //  Assert.IsNotNull( controller != null );
            //assert(controller != null);

            long waitInterval = 2000000; // microseconds = 2 sec
            long startOfWaitInterval = 0;

            bool waiting = false;

            while (true)
            {

                // Thread.sleep(2000);


                Frame frame = controller.Frame();


                if (!waiting)
                {

                    if (!frame.Hands.IsEmpty)
                    {

                        Hand currenthand = frame.Hands[0];

                        Finger thumb = frame.Fingers.FingerType(Finger.FingerType.TYPE_THUMB)[0];


                        foreach (Hand temp in frame.Hands)
                        {


                            if (temp.IsRight)
                            {
                                currenthand = temp;
                            }


                            else if (temp.IsLeft && temp.Fingers.Extended().Count == 1 && thumb.IsExtended == true)
                            {

                                Console.WriteLine("Do you want to Exit ? (Y/N)");

                                if (temp.PalmNormal.Roll <= 2.2609854)
                                    if ((temp.PalmNormal.Roll >= 0.8613482) == temp.IsValid)
                                    { // thumb up
                                        //Console.WriteLine("______________________________________________________________");

                                        //Console.WriteLine(": النص النهائي");
                                        //Console.WriteLine(currentWord);
                                        txt1.Text = currentWord;
                                        //Console.WriteLine("---------------------| تم إغلاق البرنامج |----------------------------");

                                        controller.RemoveListener(listener);
                                        Environment.Exit(0);

                                    }
                                    else if (temp.PalmNormal.Roll <= -0.2662251)
                                        if ((temp.PalmNormal.Roll >= -2.0956128) == temp.IsValid)
                                        { // thumb down
                                            //Console.WriteLine("---------------------| استمرار |----------------------------");
                                            break;

                                        }
                            }
                        }


                        // كود اخد الداتا من الليب موشن
                        double[] gesture = new double[21];
                        //double[] gesture1 = new double[21];

                        Vector Palm = currenthand.Direction;
                        // out.println("ٌRoll: "+currenthand.palmNormal().roll());

                        gesture[20] = (Math.Abs(currenthand.PalmNormal.Roll) > Math.PI / 4 ? 1 : 0);
                        //gesture1[20] = currenthand.palmNormal().roll();

                        // System.out.println("gesture(20): "+gesture[20]);
                        int fingernum = 0;

                        foreach (Finger x in currenthand.Fingers)
                        {


                            gesture[15 + fingernum] = x.IsExtended ? 1 : 0;

                            //	gesture1[15 + fingernum] = x.isExtended() ? 1 : 0;

                            for (int u = 0; u < 3; u++)
                            {

                                //مهم لحفظ حركات جديدة

                                switch (u)
                                {
                                    case 0:
                                        gesture[fingernum * 3 + 0] = x.Direction.x - Palm.x;
                                        continue;

                                    case 1:
                                        gesture[fingernum * 3 + 1] = x.Direction.y - Palm.y;
                                        continue;

                                    case 2:
                                        gesture[fingernum * 3 + 2] = x.Direction.z - Palm.z;
                                        continue;

                                }


                            }

                            fingernum++;
                        }


                        if (SHOW_SERIALIZED_GUESTURE)
                        {


                            string newGesture = string.Join(",", gesture.Select(x => x.ToString()).ToArray());

                            Console.Write("Serialized Guesture:\n" + "[" + newGesture + "]");


                        }
                        // out.println("Serialized Guesture - Roll:\n" +
                        // Arrays.toString(gesture1));

                        /*for (int i = 0; i < gesture.length; i++) {

                            bf.write(gesture[i] + ",");

                        }

                        for (int i = 0; i < gesture1.length; i++) {

                            bf2.write(gesture1[i] + ",");

                        }*/

                        // bf.write("\n");
                        // bf.write("Serialized Guesture - Roll:\n" +
                        // Arrays.toString(gesture1));
                        // bf.write("\n");

                        // the current frame time .
                        startOfWaitInterval = controller.Now();

                        waiting = true;

                        string match;

                        // match = bestMatchArithmetic(perfectLetters, gesture, bf, file);

                        match = bestMatchArithmetic(result, gesture);

                        addtoword(match);



                    }

                }



                // this code meaning subtract Past frames with current frame to know if the interval is past
                // if the result is greater than interval which is 2 sec then the interval is past  
                // so the program will continue 
                else if (frame.Timestamp - startOfWaitInterval > waitInterval)
                {

                    waiting = false;



                }
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {
        
        }

        // Methods

        public string bestMatchArithmetic(Dictionary<string, double[]> set, double[] match)
        {


            // double[] doubleArray = set.Select<string[],double>(s => Double.Parse(s)).ToArray<double>();


            string bestmatch = "";
            double[] overalfingerdiffs = new double[5];
            double bestDiff = long.MaxValue;
            double overalworstfinger = -1;


            foreach (string key in set.Keys)
            {

                if (set[key][20] != match[20])
                {
                    continue;
                }
                double[] fingerdiffs = new double[5];
                double highestFingerDiff = 0;
                double worstFinger = -1;

                for (int x = 0; x < 5; x++)
                {
                    double diff = 0;
                    for (int i = 0; i < 3; i++)
                    {

                        if (set[key][i] * match[i] > 0)
                        {
                            diff += Math.Pow(Math.Abs(set[key][x * 3 + i] - match[x * 3 + i]), 5);
                        }
                        else
                        {
                            diff += Math.Pow(Math.Abs(set[key][x * 3 + i] + match[x * 3 + i]), 5);
                        }
                    }
                    fingerdiffs[x] = diff;
                    if (diff > highestFingerDiff)
                    {
                        highestFingerDiff = diff;
                        worstFinger = x;
                    }
                }
                if (highestFingerDiff < bestDiff)
                {
                    bestDiff = highestFingerDiff;

                    bestmatch = key;
                    overalworstfinger = worstFinger;
                    overalfingerdiffs = fingerdiffs;
                }
            }
            // System.out.println("Best match: "+bestmatch+">>>"+"Best Diffrent:
            // "+bestDiff);

            /*bf.write(bestmatch + "," + bestDiff);
            bf.write("\n");*/

            if (bestmatch.Equals("#"))
            {

                backspace();
            }
            return bestmatch;
        }

        public string currentWord = "";

        public void addtoword(string in1)
        {
            if (in1.Equals("@"))
            {
                currentWord += in1.Replace("@", " ");
                Console.WriteLine();
                txt1.Text = "\"" + currentWord + "\"";

            }
            else if (in1.Equals("#"))
            {
                currentWord += in1.Replace("#", "");
                txt1.Text = currentWord;

            }
            else
            {
                currentWord += in1.Replace(" ", "");
                //Console.WriteLine("\"" + currentWord + "\"");
                txt1.Text = "\"" + currentWord + "\"";
            }

        }


        public void backspace()
        {

            if (currentWord.Length > 0)
            {
                currentWord = currentWord.Substring(0, currentWord.Length - 1);
            }
            if (currentWord.Length == 0)
            {
                //Console.WriteLine("لا يوجد حرف لحذفه");
            }
            else
            {
                //Console.WriteLine("\"" + currentWord + "\"");
                txt1.Text = "\"" + currentWord + "\"";
            }

        }

        public void enter()
        {

            //Console.WriteLine("______________________________________________________________");
            //Console.WriteLine(": النص النهائي");
            //Console.WriteLine(currentWord);
            txt1.Text = currentWord;
            //try
            //{
            //}
            //catch (Exception e)
            //{
            //    //Console.WriteLine(e.ToString());
            //    //Console.Write(e.StackTrace);
            //}
            //Console.WriteLine("---------------------| تم حذف المحتوى |----------------------------");
            currentWord = "";
            txt1.Text = currentWord;


        }



    }
}
