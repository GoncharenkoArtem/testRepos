// See https://aka.ms/new-console-template for more information




using Microsoft.VisualBasic;
using project_1;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;






int[] MyArr3  = { 1, 2, 3 , 5, 3, 1, 0, -2, 4};
Array.Sort(MyArr3); 
int bb = MyArr3.Where(i=>i >3).Sum();

Console.WriteLine(bb);

for (int i = 0; i < MyArr3.Length; i++)
{
    //Console.WriteLine(MyArr3[i]);
}






//Environment.Exit(0);






//static int Sum(int t = 0, int f = 0)
//{
//    return 0;
//}





//while (true) {

//        Console.Clear();
//        int a, b;
//        a = 1;

//        try
//        {
//            Console.WriteLine( "Введи что-нибудь :");
//            a = int.Parse(Console.ReadLine());
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine("Не то"); 
//            Console.ReadLine();
//            continue;
//        }
//                int j = 5;
//                do
//                {
//                    j--;
//                    Console.WriteLine( " do while итерация  " + j);
//                } while  (j>0);

//    for (int i = 0, u = 5, k = 9; i < 5; i++) { }

//        switch (a)
//        {
//            case 1:
//                Console.WriteLine("ok");
//                break;                  

//            default:
//                Console.WriteLine("default");
//                break;                    
//        }

//        Console.ReadLine();
//}



string ll = "введи чилсло" + "\t";
Console.Write(ll);

int kol = 0;


try
{
  kol = int.Parse(Console.ReadLine());
}
catch (Exception)
{
    Console.WriteLine("авария");
    Environment.Exit(0);
}


Console.WriteLine(kol);


//Environment.Exit(0);


//while (true) {

//Console.WriteLine(gg(kol));

// ggg(10);

//   Console.WriteLine(l l +"Привеn" + ll + i);
//   Console.WriteLine(i);
//}



int[] Myarr = new int[10];
int a = ll.Length;


for (int i = 0; i < Myarr.Length; i++) 
{ 
    Myarr[i] =(int) Math.Pow(i, 3);
    Console.WriteLine(Myarr[i]);
}



static void ff(int[]Arr,byte y = 0)
{
    for (int i = 0; i < Arr.GetLength(0); i++) {
        Console.WriteLine( "dasfdsfsdfdsf");
    }
}



ff(Myarr, 2);


Console.WriteLine(Myarr[2]);
Array.Resize(ref Myarr, 202);
Console.WriteLine(Myarr[2]);

//Environment.Exit(0);    


int v = 0;

while (v<10)
{
   
    v++;

    Console.WriteLine(" privet " + v);
  
}



/// <summary>
/// 
/// </summary>



//int[] Myarr_2 = new int[Myarr.Length];
//for (int i = 0; i < Myarr.Length; i++)
//{
//    Myarr_2[Myarr.Length - 1 - i] = Myarr[i];
//

//Myarr = Myarr_2;
//for (int i = 0; i < Myarr.Length; i++) Console.WriteLine($"xbckj {Myarr[i]}:\t" );

//int d = 4;
//Myarr_2 = Myarr[..d];


#if DEBUG

Console.WriteLine(Array.IndexOf(Myarr, 27));
Console.WriteLine(Myarr.Where(i => i == 27).Count());


//Console.WriteLine(Myarr.Where(i => i>0).Sum());
//Console.WriteLine(Myarr.Min());


//for (int i = 0; i < Myarr_2.Length; i++) Console.WriteLine($"рпопропро {Myarr_2[i]}:\t");
//Console.ReadLine();

#endif

/*
//Myarr [0] = "wer";
Console.WriteLine(Myarr.Length);
//Array.Resize(ref Myarr,20);
Array.Resize(ref Myarr, 25);
Console.WriteLine(a);
*/



ll = (true == false ? "trt" : "rtrret");
Console.WriteLine(ll);

Class1 ck = new Class1();
Console.WriteLine(ck.HelloHabr(29));


Ass rtt = new Ass();
rtt.Pee();


string str = null;
str ??= "ОЙ ОЙ ОЙ"; 
Console.WriteLine(str);


internal class Ass
{


    public string Pee()
    {
        Console.WriteLine("PEE-PEE");
        return "";
    }



    /// <summary>
    /// This property always returns a value &lt; 1.
    /// </summary>
    static int Sum(int t = 0)
    {
        return 0;
    }






















}











