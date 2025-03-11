using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BrowsersManager.Models
{
    public class BrowserType
    {
        public string Name { get; set; }
        public string Path { get; set; }
        
        // 静态方法：加载所有浏览器类型
        public static List<BrowserType> LoadAllBrowserTypes()
        {
            List<BrowserType> browserTypes = new List<BrowserType>();
            try
            {
                if (File.Exists("BrowserType.json"))
                {
                    string jsonContent = File.ReadAllText("BrowserType.json");
                    browserTypes = JsonConvert.DeserializeObject<List<BrowserType>>(jsonContent);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading browser types: {ex.Message}", ex);
            }
            
            return browserTypes;
        }
        
        // 静态方法：保存所有浏览器类型
        public static void SaveAllBrowserTypes(List<BrowserType> browserTypes)
        {
            try
            {
                string jsonContent = JsonConvert.SerializeObject(browserTypes, Formatting.Indented);
                File.WriteAllText("BrowserType.json", jsonContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving browser types: {ex.Message}", ex);
            }
        }
        
        // 静态方法：按名称查找浏览器类型
        public static BrowserType FindByName(string name)
        {
            var browserTypes = LoadAllBrowserTypes();
            return browserTypes.Find(b => b.Name == name);
        }
    }
} 