using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Text;
using wpf_test;
using System.Drawing;
using System.Windows;
using System.Windows.Navigation;
using System.Numerics;
using System.Windows.Controls;
using System.Collections.ObjectModel;

public class PDF_Creator
{
    private static string path_exe = Environment.CurrentDirectory;
    private static DirectoryInfo path_pdf = Directory.CreateDirectory(Path.Combine(path_exe, "Temp_PDF"));  // создаем папку с временным хранением файлов PDF
    private ObservableCollection<FileData> _list_files;
    private ObservableCollection<UserData> _list_users;
    private List<string> _listval;
    private iTextSharp.text.Font mainFont;
    private iTextSharp.text.Font smallFont;
    private PdfWriter writer;


    public PDF_Creator(ObservableCollection<FileData> list_files, ObservableCollection<UserData> list_users, List<string> listval)
    {
        _list_files = list_files;
        _list_users = list_users;
        _listval = listval;

        // Регистрируем провайдер кодировок для поддержки кодировок Windows
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // Путь к шрифту TTF 
        string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
        BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

        // Используем поле класса вместо локальной переменной
        mainFont = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.NORMAL);
        smallFont = new iTextSharp.text.Font(bf, 3, iTextSharp.text.Font.NORMAL);
    }




    public void GeneratePdf(int type)
    {
        DeleteAllPDF(); // очищаем папку Temp

        iTextSharp.text.Document pdfDoc = null;
        
        if (_list_files.Count == 0 || _list_users.Count == 0) { return; }
        
        pdfDoc = NewPDF();
        pdfDoc.Open();


        switch (type)
        {
            case 1:  // многостраничный вариант

                for (byte i = 0; i < _list_files.Count; i++)
                {
                    pdfDoc.NewPage();
                    Frame(); // Добавляем рамку на лист
                    AddParagraph(pdfDoc, 2, mainFont); // Отступаем абзацы  
                    PDF_type_1(pdfDoc, i);  // Делаем таблицу тип 1 (по одному документу на странице)
                }
                break;

            case 2: // одностраничный вариант
               
                Frame(); // Добавляем рамку на лист
                AddParagraph(pdfDoc, 2, mainFont); // Отступаем абзацы  
                PDF_type_2(pdfDoc);  // Делаем таблицу тип 2 (на одной странице)
                break;
        }
        pdfDoc.Close(); // Закрываем документ
        pdfDoc.Dispose();

    }





    private void PDF_type_1(iTextSharp.text.Document _pdfDoc, byte index) // Создадим таблицу ИУЛ тип 1 (по одному документу на странице)
    {
        PdfPTable table = new PdfPTable(4); // Создаем таблицу с 4 колонками
        table.WidthPercentage = 100; // ширина таблицы на всю страницу
        table.SetWidths(new float[] { 0.8f, 2f, 3.5f, 2f }); // ширина колонок

        _pdfDoc.Add(new Paragraph(_list_files.Count));

        // Добавляем шапку таблицы 1
        table.AddCell(GetCell("Номер п/п", mainFont));
        table.AddCell(GetCell("Обозначение документа", mainFont));
        table.AddCell(GetCell("Наименование изделия Наименование документа", mainFont));
        table.AddCell(GetCell("Номер последнего изменения (версии)", mainFont));
       
        // Добавляем данные в таблицу 1
        if (_list_files.Count > 1) // если первый лист и листов будет не один
        {
            table.AddCell(GetCell((index + 1).ToString(), mainFont));
        }
        else
        {  // только один лист в выпуске ИУЛ
            table.AddCell(GetCell(" ", mainFont));
        }

        table.AddCell(GetCell(_listval[1], mainFont)); //   Обозначение документа
        table.AddCell(GetCell(_listval[2], mainFont)); //   Наименование изделия Наименование документа
        table.AddCell(GetCell(_listval[0], mainFont)); //   Номер последнего изменения
        _pdfDoc.Add(table);

        AddParagraph(_pdfDoc,1,smallFont);
        PdfPTable table2 = new PdfPTable(2); // Создаем таблицу с 2 колонками
        table2.WidthPercentage = 100; // ширина таблицы на всю страницу
        table2.SetWidths(new float[] { 2.8f, 5.5f }); // ширина колонок
        table2.AddCell(GetCell("CRC32", mainFont));   
        table2.AddCell(GetCell(_list_files[index].File_Sum ?? " ", mainFont)); //  Контрольная сумма 
        _pdfDoc.Add(table2);

        AddParagraph(_pdfDoc, 1, smallFont);
        PdfPTable table3 = new PdfPTable(3); // Создаем таблицу с 3 колонками
        table3.WidthPercentage = 100; // ширина таблицы на всю страницу
        table3.SetWidths(new float[] { 2.8f, 2.5f, 3f }); // ширина колонок
        table3.AddCell(GetCell("Наименования файла", mainFont));  
        table3.AddCell(GetCell("Дата и время последнего изменения файла", mainFont));  
        table3.AddCell(GetCell("Размер файла, байт", mainFont)); 
        table3.AddCell(GetCell(_list_files[index].File_Name ?? " ", mainFont)); //   Имя документа
        table3.AddCell(GetCell(_list_files[index].File_Date ?? " ", mainFont)); //   Дата изменения
        table3.AddCell(GetCell(_list_files[index].File_Size.ToString() ?? "0", mainFont)); //   Размер файла
        _pdfDoc.Add(table3);

        AddParagraph(_pdfDoc, 1, smallFont);
        PdfPTable table4 = new PdfPTable(4); // Создаем таблицу с 3 колонками
        table4.WidthPercentage = 100; // ширина таблицы на всю страницу
        table4.SetWidths(new float[] {1f,1f,1f,1f}); // ширина колонок
        table4.AddCell(GetCell("Характер работы", mainFont));  
        table4.AddCell(GetCell("Фамилия", mainFont)); 
        table4.AddCell(GetCell("Подпись", mainFont));
        table4.AddCell(GetCell("Дата подписания", mainFont));

        for (int i = 0; i < _list_users.Count; i++)
        {
            table4.AddCell(GetCell(_list_users[i].SelectedPosition_pos ?? " ", mainFont)); // Должность
            table4.AddCell(GetCell(_list_users[i].Second_name ?? " ", mainFont)); // Фамилия
            table4.AddCell(GetCell(" ", mainFont)); // Просто пустая ячейка для подписи
            table4.AddCell(GetCell(_list_users[i].Date ?? " " , mainFont));
        }
        _pdfDoc.Add(table4);

        GetSigns(_pdfDoc, table4);
        MainFrame(1,index);  // в качестве параметра передаем тип ИУЛа номер документа
    }


    private void PDF_type_2(iTextSharp.text.Document _pdfDoc) // Создадим таблицу ИУЛ тип 2 (несколько документов на одной странице)
    {
        PdfPTable table = new PdfPTable(4); // Создаем таблицу с 4 колонками
        table.WidthPercentage = 100; // ширина таблицы на всю страницу
        table.SetWidths(new float[] { 0.8f, 2f, 3.5f, 2f }); // ширина колонок

        _pdfDoc.Add(new Paragraph(_list_files.Count));

        // Добавляем шапку таблицы 1
        table.AddCell(GetCell("Номер п/п", mainFont));
        table.AddCell(GetCell("Обозначение документа", mainFont));
        table.AddCell(GetCell("Наименование изделия Наименование документа", mainFont));
        table.AddCell(GetCell("Номер последнего изменения (версии)", mainFont));

        table.AddCell(GetCell(" ", mainFont));  // только один лист в выпуске ИУЛ
        table.AddCell(GetCell(_listval[1], mainFont)); //   Обозначение документа
        table.AddCell(GetCell(_listval[2], mainFont)); //   Наименование изделия Наименование документа
        table.AddCell(GetCell(_listval[0], mainFont)); //   Номер последнего изменения
        _pdfDoc.Add(table);

        AddParagraph(_pdfDoc, 1, smallFont);
        PdfPTable table2 = new PdfPTable(2); // Создаем таблицу с 2 колонками
        table2.WidthPercentage = 100; // ширина таблицы на всю страницу
        table2.SetWidths(new float[] { 2.8f, 5.5f }); // ширина колонок
        table2.AddCell(GetCell("CRC32", mainFont));
        table2.AddCell(GetCell(" ", mainFont)); //  Пустая ячейка, тут должна была быть контрольная сумма, но мы ее добавим ниже в таблице
        _pdfDoc.Add(table2);

        AddParagraph(_pdfDoc, 1, smallFont);
        PdfPTable table3 = new PdfPTable(4); // Создаем таблицу с 3 колонками
        table3.WidthPercentage = 100; // ширина таблицы на всю страницу
        table3.SetWidths(new float[] { 2.8f, 2f, 1.75f, 1.75f }); // ширина колонок
        table3.AddCell(GetCell("Наименования файла", mainFont));
        table3.AddCell(GetCell("Дата и время последнего изменения файла", mainFont));
        table3.AddCell(GetCell("Размер файла, байт", mainFont));
        table3.AddCell(GetCell("Значение контрольной суммы", mainFont));

        for (byte i = 0; i < _list_files.Count; i++)
        {
            table3.AddCell(GetCell(_list_files[i].File_Name ?? " ", mainFont)); //   Имя документа
            table3.AddCell(GetCell(_list_files[i].File_Date ?? " ", mainFont)); //   Дата изменения
            table3.AddCell(GetCell(_list_files[i].File_Size.ToString() ?? "0", mainFont)); //   Размер файла
            table3.AddCell(GetCell(_list_files[i].File_Sum ?? " ", mainFont)); //  Контрольная сумма
        }
        _pdfDoc.Add(table3);

        AddParagraph(_pdfDoc, 1, smallFont);
        PdfPTable table4 = new PdfPTable(4); // Создаем таблицу с 3 колонками
        table4.WidthPercentage = 100; // ширина таблицы на всю страницу
        table4.SetWidths(new float[] { 1f, 1f, 1f, 1f }); // ширина колонок
        table4.AddCell(GetCell("Характер работы", mainFont));
        table4.AddCell(GetCell("Фамилия", mainFont));
        table4.AddCell(GetCell("Подпись", mainFont));
        table4.AddCell(GetCell("Дата подписания", mainFont));

        for (int i = 0; i < _list_users.Count; i++)
        {
            table4.AddCell(GetCell(_list_users[i].SelectedPosition_pos ?? " ", mainFont)); // Должность
            table4.AddCell(GetCell(_list_users[i].Second_name ?? " ", mainFont)); // Фамилия
            table4.AddCell(GetCell(" ", mainFont)); // Просто пустая ячейка для подписи
            table4.AddCell(GetCell(_list_users[i].Date ?? " ", mainFont));
        }
        _pdfDoc.Add(table4);
        GetSigns(_pdfDoc, table4);
        MainFrame(2, 0);  // в качестве параметра передаем тип ИУЛа номер документа
    }



    // Вспомогательный метод для создания ячейки
    private PdfPCell GetCell(string text, iTextSharp.text.Font font)
    {
        PdfPCell cell = new PdfPCell(new Phrase(text, font))
        {
            HorizontalAlignment = PdfPCell.ALIGN_CENTER,
            VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
            Padding = 5,
            BorderWidth = 1
        };
        return cell;
    }

    // Вспомогательный метод для создания ячейки, объединенной по строкам
    private PdfPCell GetMergedRowCell(string text, iTextSharp.text.Font font, byte num)
    {
        PdfPCell cell = new PdfPCell(new Phrase(text, font))        
        {
            HorizontalAlignment = PdfPCell.ALIGN_CENTER,
            VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
            Padding = 5,
            BorderWidth = 1
        };
        cell.Rowspan = num;
        return cell;
    }



    // Вспомогательный метод для создания ячейки, объединенной по колонкам
    private PdfPCell GetMergedColumnCell(string text, iTextSharp.text.Font font, byte num)
    {
        PdfPCell cell = new PdfPCell(new Phrase(text, font))
        {
            HorizontalAlignment = PdfPCell.ALIGN_CENTER,
            VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
            Padding = 5,
            BorderWidth = 1
        };
        cell.Colspan = num;
        return cell;
    }


    private void MainFrame(byte type, byte index)
    {
        PdfPTable table = new PdfPTable(4); // Создаем таблицу с 3 колонками
        table.TotalWidth = PageSize.A4.Width - 56.67f -  14.17f // ширина листа минус рамка

        ; // ширина таблицы на всю страницу
        table.SetWidths(new float[] { 2.8f, 3.9f, 0.8f, 0.8f }); // ширина колонок
        table.AddCell(GetMergedRowCell("Информационно-удостоверяющий лист", mainFont, 2));

        if (_listval[1] != "")
        {
            table.AddCell(GetMergedRowCell(_listval[1] + "-УЛ", mainFont, 2));
        }
        else 
        {
            table.AddCell(GetMergedRowCell(_listval[1] + " ", mainFont, 2));
        }

        table.AddCell(GetCell("Лист", mainFont));
        table.AddCell(GetCell("Листов", mainFont));


        if (type != 1)
        {
            table.AddCell(GetCell(" ", mainFont));
            table.AddCell(GetCell(" ", mainFont));
        }else{

            if (index > 0) // если листов больше одного и это уже не первый лист ИУЛ
            {
                table.AddCell(GetCell((index + 1).ToString(), mainFont));
                table.AddCell(GetCell(" ", mainFont));
            }else{ // если это всетаки первый лист
                if (_list_files.Count > 1) // если первый лист и листов будет не один
                {
                    table.AddCell(GetCell((index + 1).ToString(), mainFont));
                    table.AddCell(GetCell(_list_files.Count.ToString(), mainFont));
                }else{  // только один лист в выпуске ИУЛ
                    table.AddCell(GetCell(" ", mainFont));
                    table.AddCell(GetCell(" ", mainFont));
                }
            }
        }

        // Задаем координаты для таблицы
        float x = 56.67f; // Координата X (слева направо)
        float y = 14.17f + table.TotalHeight; // Координата Y (снизу вверх)

        // Добавляем таблицу на страницу по указанным координатам
        table.WriteSelectedRows(0, -1, x, y, writer.DirectContent);
    }



    private void Frame() // метод создания рамки 
    {
        // Получаем PdfContentByte из PdfWriter
        PdfContentByte canvas = writer.DirectContent;
        // Задаем координаты и размеры прямоугольника
        canvas.Rectangle(56.69f, 14.17f, 524.41f, 813.54f);
        // Устанавливаем цвет границы
        canvas.SetColorStroke(BaseColor.BLACK);
        // Толщина границы
        canvas.SetLineWidth(2f);
        // Заливаем прямоугольник цветом и рисуем его
        canvas.Stroke();
    }


    private void AddParagraph(iTextSharp.text.Document _pdfDoc, int count, iTextSharp.text.Font _font) // добавить пустой абзац
    {
        for (int i = 0; i < count; i++) { _pdfDoc.Add(new Paragraph(" ", _font)); }
    }


    private void DeleteAllPDF() // удаляем все в папке Temp 
    {
        var folder = new DirectoryInfo(path_pdf.ToString());
        foreach (FileInfo file in folder.GetFiles())
        {
            file.Delete();
        }
    }


    private iTextSharp.text.Document NewPDF()  // Создаем новый файл пдф
    {
        // создаем файл PDF (в имени порядковый номер и дата/время создания)
        int count = Directory.GetFileSystemEntries(path_pdf.FullName).Length;
        //DateTime now = DateTime.Now;
        //string pdfPath = path_pdf.FullName + "\\" + (count + 1) + "_IUL_PDF_" + now.ToString("dd.MM.yy") + ".pdf"; 
        string pdfPath = path_pdf.FullName + "\\Temp_IUL_PDF.pdf"; 
        iTextSharp.text.Document _pdfDoc = new iTextSharp.text.Document(PageSize.A4, 56.69f, 14.17f, 14.17f, 14.17f);
        writer = PdfWriter.GetInstance(_pdfDoc, new FileStream(pdfPath, FileMode.Create));
        return _pdfDoc;
    }




    public float GetTableHeight(PdfPTable _table)
    {
        float totalHeight = 0;
        // Проходим по каждой строке в таблице и суммируем их высоту
        for (int rowIndex = 0; rowIndex < _table.Rows.Count; rowIndex++)
        {
            PdfPRow row = _table.Rows[rowIndex];
            totalHeight += row.MaxHeights; // MaxHeights возвращает максимальную высоту строки
        }
        return totalHeight;
    }







    private void  GetSigns(iTextSharp.text.Document _pdfDoc,PdfPTable _table) 
    {
        //iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance("x:\\08_ОБМЕН МЕЖДУ СОТРУДНИКАМИ\\Гончаренко\\Переезд_2\\Подписи\\Абрамова.gif");

        if (_table != null && _table.Rows.Count > 0)
        {
            //float coord_Y = writer.GetVerticalPosition(true) - _table.Rows[_table.Rows.Count - 1].GetCells()[0].Height + GetTableHeight(_table) - _table.Rows[0].GetCells()[0].Height / 2; // Позиция Y курсора  - последняя строка (курсор не переходит с последней строки, пока нет новых данных)+ высота таблицы - высота шапки таблицы = координаты первой строки таблицы
            //float coord_X = 0;
            
            float coord_Y = writer.GetVerticalPosition(true)  + GetTableHeight(_table) - _table.Rows[0].GetCells()[0].Height/2   ; // Позиция Y курсора  - последняя строка (курсор не переходит с последней строки, пока нет новых данных) + высота таблицы - половина высоты шапки таблицы  = координата для первой подписи
            float coord_X = 0;
           
            
            for (int i = 1; i < _table.Rows.Count; i++)
            {
                coord_Y = coord_Y - _table.Rows[i - 1].GetCells()[0].Height/2 - _table.Rows[i].GetCells()[0].Height/2 ; // половина высоты предыдущей строки минус половина высоты текущей строки
                coord_X = i%2==0 ? 365 : 405;  // для шахматного порядка вставки подписей (385 - это середина ячейки)
               
                // Кол-во строк в таб0лице будет равно кол-ву объектов коллекции _List_users. Проходимся и смотрим где есть подписи, если есть галочка, то вытаскиваем картинку
                //масштабируем ее и вставляем в ПДФ с высчитанными координатами

                if (_list_users[i-1].Sign) // Если стоит галочка для подписи
                {
                    if (File.Exists(_list_users[i-1].SignPath)) // проверяем путь
                    {
                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(_list_users[i-1].SignPath);
                        img = GetImageSize(img); // масштабируем в нужный формат
                        img.SetAbsolutePosition(coord_X - img.ScaledWidth/2, coord_Y- img.ScaledHeight/2); // отнимаем половину высоты и ширины картинки подписи, чтобы она была по центру ячеи
                        _pdfDoc.Add(img);

                    }
                }
            }
        }
    }


    private iTextSharp.text.Image GetImageSize(iTextSharp.text.Image image)
    {
        float x_dim = image.ScaledWidth;
        float y_dim = image.ScaledHeight;

        // ограничивающую рамку для подписи принимаем 80 (длина) х 40 (ширина). Исходя из этого высчитаем пропорции  масштабирования картинки
        float k_x = 70 / (float)x_dim;
        float k_y = 30 / (float)y_dim;

        // найдем минимальный коэфициент 
        float k =Math.Min(k_x, k_y);

        // масштабируем картинку
        image.ScaleAbsolute(x_dim*k_x, y_dim*k_y);  

        return image;
    }







}









