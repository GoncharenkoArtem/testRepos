
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace project_1
{
    public class User
    {
        public int id { get; set; }
        public string? first_name { get; set; } = "нет данных";
        public string? second_name { get; set; } = "нет данных";
        public string? patronymic_name { get; set; } = "нет данных";
        public string? short_FIO
        {
            get {
                string? temp_second_name = (second_name != "нет данных" && second_name != "") ? second_name : "";
                string? temp_first_name = (first_name != "нет данных" && first_name != "") ? first_name?.Substring(0, 1) + "." : "";
                string? temp_patronymic_name = (patronymic_name != "нет данных" && patronymic_name != "") ? patronymic_name?.Substring(0,1) + "." : "";
                return $"{temp_second_name} {temp_first_name} {temp_patronymic_name}"; }
            set {}
        }

        public string? position { get; set; } = "нет данных";
        public string? image_path { get; set; } = "нет данных";

    }

}