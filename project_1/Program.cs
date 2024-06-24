// See https://aka.ms/new-console-template for more information

using Force.Crc32;
using project_1;
using static System.Net.WebRequestMethods;
using System.Reflection.Metadata;
using System.Drawing;
//DB_users DB = new DB_users();


{

    //Image newImage = Image.FromFile("SampImag.jpg");

    //canvas.AddImage()
    //    // Устанавливаем цвет границы
    //    canvas.SetColorStroke(BaseColor.BLACK);
    //// Толщина границы
    //canvas.SetLineWidth(2f);
    //// Заливаем прямоугольник цветом и рисуем его
    //canvas.Stroke();
}








//User user = new User { first_name = "Артем", second_name = "Гончаренко", patronymic_name = "Эдуардович", position = "оссенизатор", };
//DB.Add(user);
//User user1 = new User { first_name = "Мария", second_name = "Воробъева", patronymic_name = "Александровна", position = "криптозоолог", };
//DB.Add(user1);
//User user2 = new User { first_name = "Александр", second_name = "Коваленко", patronymic_name = "Николаевич", position = "нейролингвист", };
//DB.Add(user2);
//User user3 = new User { first_name = "Александра", second_name = "Басова", patronymic_name = "Сергеевна", position = "страусиная няня", };
//DB.Add(user3);
//User user4 = new User { first_name = "Тимур", second_name = "Базаров", patronymic_name = "Салаватович", position = "пандаукладчик", };
//DB.Add(user4);


//db_2.Clear();


//DB_users.AddAllData(user);
//DB.DeleteID(8);
//DB.DeleteID(29);
//DB.Clear();
//var bb = DB_users.users.ToList();





//var db = DB.GetID(52);




//  string filePath = @"I:\05_ОТДЕЛЫ\ИСКУССТВЕННЫЕ СООРУЖЕНИЯ\_Проекты\2206 Кв4 Новосергиево\РД\Трубы\Неред\Вер2\0_Титул_АД-01-ВТ.pdf"; // Замените на путь к вашему файлу

// Вычисление CRC32 контрольной суммы
// string crc32Checksum = ComputeCrc32(filePath);

// Преобразование байтового массива в строку


//uint crc32Checksum = ComputeCrc32(filePath);

// Преобразование контрольной суммы в строку
// string checksumString = crc32Checksum.ToString("X8");

// Console.WriteLine($"CRC32 Checksum: {crc32Checksum}");


//string checksumString = BitConverter.ToString(crc32Checksum).Replace("-", "").ToLower();
//Console.WriteLine($"CRC32 Checksum: {checksumString}");
//Console.WriteLine($"CRC32 Checksum: {checksumString}");


//static uint ComputeCrc32(string filePath)
//{
//    using (FileStream stream = File.OpenRead(filePath))
//    {
//        return Crc32Algorithm.Compute(stream);
//    }
//}


//static string ComputeCrc32(string filePath)
//{
//    if (File.Exists(filePath))
//    {
//        using (FileStream stream = File.OpenRead(filePath))
//        {
//            byte[] fileBytes = File.ReadAllBytes(filePath);
//            var crc32 = new Crc32CAlgorithm();
//            byte[] hash = crc32.ComputeHash(fileBytes);
//            return BitConverter.ToString(hash).Replace("-", "").ToUpper();
//        }
//    }
//    return null ;
//}
