// See https://aka.ms/new-console-template for more information



using Microsoft.VisualBasic;
using project_1;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

//int[] MyArray = new int[5];

int[] MyArray = { 0, 15, 12, 78, 9, -5, 44 };


static void Printarr(int[] arr,int pos)
{
    int sum = 0;

    if (pos == arr.Length) return;
    Console.WriteLine(sum += arr[pos]);
    pos++;

    Printarr(arr, pos);
}

Printarr(MyArray, 0);








