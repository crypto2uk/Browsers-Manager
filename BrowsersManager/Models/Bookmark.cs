using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BrowsersManager.Models
{
    /// <summary>
    /// Represents a bookmark with name, URL and timestamp information
    /// </summary>
    public class Bookmark
    {
        /// <summary>
        /// The name of the bookmark
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The URL of the bookmark
        /// </summary>
        public string Url { get; set; }
        
        /// <summary>
        /// The timestamp when the bookmark was added or last updated
        /// </summary>
        public DateTime AddedTime { get; set; }

        /// <summary>
        /// The file path for the bookmark storage
        /// </summary>
        private static readonly string BookmarkFilePath = "bookmark.json";

        /// <summary>
        /// Loads all bookmarks from the JSON file
        /// </summary>
        /// <returns>A list of bookmarks</returns>
        public static List<Bookmark> LoadAllBookmarks()
        {
            List<Bookmark> bookmarks = new List<Bookmark>();
            try
            {
                if (File.Exists(BookmarkFilePath))
                {
                    string jsonContent = File.ReadAllText(BookmarkFilePath);
                    bookmarks = JsonConvert.DeserializeObject<List<Bookmark>>(jsonContent) ?? new List<Bookmark>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading bookmarks: {ex.Message}");
                // Return empty list in case of error
            }
            
            return bookmarks;
        }

        /// <summary>
        /// Saves all bookmarks to the JSON file
        /// </summary>
        /// <param name="bookmarks">The list of bookmarks to save</param>
        public static void SaveAllBookmarks(List<Bookmark> bookmarks)
        {
            try
            {
                string jsonContent = JsonConvert.SerializeObject(bookmarks, Formatting.Indented);
                File.WriteAllText(BookmarkFilePath, jsonContent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving bookmarks: {ex.Message}");
                // Just log the error for now
            }
        }

        /// <summary>
        /// Adds or updates a bookmark in the bookmark collection
        /// </summary>
        /// <param name="name">The name of the bookmark</param>
        /// <param name="url">The URL of the bookmark</param>
        /// <returns>The updated list of bookmarks</returns>
        public static List<Bookmark> AddOrUpdateBookmark(string name, string url)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(url))
            {
                return LoadAllBookmarks(); // Return existing bookmarks if input is invalid
            }

            List<Bookmark> bookmarks = LoadAllBookmarks();
            
            // Check if bookmark with the same name already exists
            Bookmark existingBookmark = bookmarks.FirstOrDefault(b => b.Name == name);
            
            if (existingBookmark != null)
            {
                // Update existing bookmark
                existingBookmark.Url = url;
                existingBookmark.AddedTime = DateTime.Now;
            }
            else
            {
                // Add new bookmark
                bookmarks.Add(new Bookmark
                {
                    Name = name,
                    Url = url,
                    AddedTime = DateTime.Now
                });
            }
            
            SaveAllBookmarks(bookmarks);
            return bookmarks;
        }

        /// <summary>
        /// Gets the most recent bookmarks, up to the specified limit
        /// </summary>
        /// <param name="limit">Maximum number of bookmarks to return</param>
        /// <returns>A list of the most recent bookmarks</returns>
        public static List<Bookmark> GetRecentBookmarks(int limit = 30)
        {
            List<Bookmark> bookmarks = LoadAllBookmarks();
            
            // Sort by added time descending and take the specified number
            return bookmarks
                .OrderByDescending(b => b.AddedTime)
                .Take(limit)
                .ToList();
        }

        /// <summary>
        /// Deletes a bookmark by its name
        /// </summary>
        /// <param name="name">The name of the bookmark to delete</param>
        /// <returns>True if the bookmark was found and deleted, false otherwise</returns>
        public static bool DeleteBookmark(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            List<Bookmark> bookmarks = LoadAllBookmarks();
            
            // Find the bookmark with the specified name
            Bookmark bookmarkToDelete = bookmarks.FirstOrDefault(b => b.Name == name);
            
            if (bookmarkToDelete != null)
            {
                // Remove the bookmark from the list
                bookmarks.Remove(bookmarkToDelete);
                
                // Save the updated list
                SaveAllBookmarks(bookmarks);
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Finds a bookmark by its name
        /// </summary>
        /// <param name="name">The name to search for</param>
        /// <returns>The bookmark if found, null otherwise</returns>
        public static Bookmark FindByName(string name)
        {
            var bookmarks = LoadAllBookmarks();
            return bookmarks.FirstOrDefault(b => b.Name == name);
        }
    }
} 