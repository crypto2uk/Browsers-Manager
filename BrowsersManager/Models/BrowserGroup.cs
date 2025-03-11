using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BrowsersManager.Models
{
    public class ProfileItem 
    {
        public string ProfileName { get; set; }
        public string Language { get; set; }
        public string UserAgent { get; set; }
        public string OtherParameters { get; set; }

        // 从逗号分隔的字符串解析
        public static ProfileItem FromString(string itemString)
        {
            string[] parts = itemString.Split(',');
            var item = new ProfileItem
            {
                ProfileName = parts[0]
            };
            
            if (parts.Length > 1) item.Language = parts[1];
            if (parts.Length > 2) item.UserAgent = parts[2];
            if (parts.Length > 3) item.OtherParameters = parts[3];
            
            return item;
        }

        // 转换为逗号分隔的字符串格式
        public override string ToString()
        {
            return $"{ProfileName},{Language},{UserAgent},{OtherParameters}";
        }
    }

    public class BrowserGroup
    {
        // Group properties
        public int Total { get; set; }
        public string BrowserType { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        
        // Profile path related properties
        public string ProfileTemplate { get; set; }
        public string UserDataDir { get; set; }
        public string ProfileRange { get; set; }
        public string CopySourceRange { get; set; }
        
        // Browser configuration properties
        public string DefaultUserAgent { get; set; }
        public string DefaultLanguage { get; set; }
        public string OtherParameters { get; set; }
        
        // Profile items - 从 string[] 改为 ProfileItem[]
        public ProfileItem[] Items { get; set; }
        
        // 静态方法：从文件加载所有分组
        public static Dictionary<string, BrowserGroup> LoadAllGroups()
        {
            Dictionary<string, BrowserGroup> groups = new Dictionary<string, BrowserGroup>();
            try
            {
                if (File.Exists("BrowserGroup.json"))
                {
                    string jsonContent = File.ReadAllText("BrowserGroup.json");
                    var jsonObj = JObject.Parse(jsonContent);
                    
                    foreach (var property in jsonObj.Properties())
                    {
                        string groupName = property.Name;
                        var groupData = property.Value;
                        
                        // 先将 items 解析为字符串数组，然后转换为 ProfileItem 对象
                        string[] itemStrings = groupData["items"].ToObject<string[]>();
                        ProfileItem[] profileItems = itemStrings.Select(ProfileItem.FromString).ToArray();

                        BrowserGroup group = new BrowserGroup
                        {
                            Name = groupName,
                            Total = (int)groupData["total"],
                            BrowserType = (string)groupData["browserType"],
                            Description = (string)groupData["description"],
                            ProfileTemplate = (string)groupData["profile_template"],
                            UserDataDir = (string)groupData["user_data_dir"],
                            ProfileRange = (string)groupData["profile_range"],
                            CopySourceRange = (string)groupData["copy_source_range"],
                            DefaultUserAgent = (string)groupData["default_user_agent"],
                            DefaultLanguage = (string)groupData["default_language"],
                            OtherParameters = (string)groupData["other_parameters"],
                            Items = profileItems
                        };
                        
                        groups.Add(groupName, group);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading browser groups: {ex.Message}", ex);
            }
            
            return groups;
        }
        
        // 静态方法：加载单个分组
        public static BrowserGroup LoadGroup(string groupName)
        {
            try
            {
                if (File.Exists("BrowserGroup.json"))
                {
                    string jsonContent = File.ReadAllText("BrowserGroup.json");
                    var jsonObj = JObject.Parse(jsonContent);
                    
                    if (jsonObj.ContainsKey(groupName))
                    {
                        var groupData = jsonObj[groupName];
                        
                        // 先将 items 解析为字符串数组，然后转换为 ProfileItem 对象
                        string[] itemStrings = groupData["items"].ToObject<string[]>();
                        ProfileItem[] profileItems = itemStrings.Select(ProfileItem.FromString).ToArray();

                        return new BrowserGroup
                        {
                            Name = groupName,
                            Total = (int)groupData["total"],
                            BrowserType = (string)groupData["browserType"],
                            Description = (string)groupData["description"],
                            ProfileTemplate = (string)groupData["profile_template"],
                            UserDataDir = (string)groupData["user_data_dir"],
                            ProfileRange = (string)groupData["profile_range"],
                            CopySourceRange = (string)groupData["copy_source_range"],
                            DefaultUserAgent = (string)groupData["default_user_agent"],
                            DefaultLanguage = (string)groupData["default_language"],
                            OtherParameters = (string)groupData["other_parameters"],
                            Items = profileItems
                        };
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading browser group '{groupName}': {ex.Message}", ex);
            }
        }
        
        // 静态方法：保存所有分组
        public static void SaveAllGroups(Dictionary<string, BrowserGroup> groups)
        {
            try
            {
                JObject jsonObj = new JObject();
                
                foreach (var pair in groups)
                {
                    string groupName = pair.Key;
                    BrowserGroup group = pair.Value;
                    
                    // 将 ProfileItem 对象转换为字符串数组
                    string[] itemStrings = group.Items.Select(item => item.ToString()).ToArray();

                    JObject groupObj = new JObject
                    {
                        ["total"] = group.Total,
                        ["browserType"] = group.BrowserType,
                        ["description"] = group.Description,
                        ["profile_template"] = group.ProfileTemplate,
                        ["user_data_dir"] = group.UserDataDir,
                        ["profile_range"] = group.ProfileRange,
                        ["copy_source_range"] = group.CopySourceRange,
                        ["default_user_agent"] = group.DefaultUserAgent,
                        ["default_language"] = group.DefaultLanguage,
                        ["other_parameters"] = group.OtherParameters,
                        ["items"] = JArray.FromObject(itemStrings)
                    };
                    
                    jsonObj[groupName] = groupObj;
                }
                
                File.WriteAllText("BrowserGroup.json", jsonObj.ToString(Formatting.Indented));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving browser groups: {ex.Message}", ex);
            }
        }
        
        // 静态方法：保存单个分组
        public static void SaveGroup(BrowserGroup group)
        {
            try
            {
                // 加载现有的分组
                Dictionary<string, BrowserGroup> groups = LoadAllGroups();
                
                // 更新或添加指定的分组
                groups[group.Name] = group;
                
                // 保存所有分组
                SaveAllGroups(groups);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving browser group '{group.Name}': {ex.Message}", ex);
            }
        }
    }
} 