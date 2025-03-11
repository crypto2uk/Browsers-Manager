using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BrowsersManager.Services
{
    /// <summary>
    /// Utility class for generating PNG images with text content.
    /// </summary>
    public class ImageGenerator
    {
        // Navy blue color (RGB: 0, 0, 128)
        private readonly Color _backgroundColor = Color.FromArgb(14, 82, 157);
        
        // Image size constants
        private const int ImageSize = 192;
        private const int HorizontalPadding = 0;
        
        /// <summary>
        /// Generates a PNG image with the specified text content.
        /// </summary>
        /// <param name="content">The text content to display in the image.</param>
        /// <param name="savePath">The file path where the image will be saved.</param>
        /// <returns>True if the image was successfully generated and saved, false otherwise.</returns>
        public bool GenerateImage(string content, string savePath)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentException("Content cannot be null or empty.", nameof(content));
            }

            if (string.IsNullOrEmpty(savePath))
            {
                throw new ArgumentException("Save path cannot be null or empty.", nameof(savePath));
            }

            try
            {
                // Create a new bitmap with the specified size
                using (Bitmap bitmap = new Bitmap(ImageSize, ImageSize))
                {
                    // Create a graphics object from the bitmap
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        // Enable high quality rendering
                        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        
                        // Fill the background with navy blue color
                        graphics.Clear(_backgroundColor);

                        // Calculate adaptive font size based on content length
                        // For 1-3 characters, use larger font sizes for fewer characters
                        float fontSize;
                        int verticalOffset; // Add vertical offset to adjust text position
                        
                        switch (content.Length)
                        {
                            case 1:
                                fontSize = 140; // Slightly reduced to prevent clipping
                                verticalOffset = 25; // Increased offset to move down
                                break;
                            case 2:
                                fontSize = 90;  // Slightly reduced
                                verticalOffset = 20; // Increased offset
                                break;
                            case 3:
                                fontSize = 66;  // Slightly reduced
                                verticalOffset = 18;  // Increased offset
                                break;
                            default:
                                fontSize = Math.Max(60, 170 / content.Length); // Adjusted fallback
                                verticalOffset = 14;  // Increased default offset
                                break;
                        }

                        // Set up text drawing properties with adaptive font size
                        using (Font font = new Font("Arial", fontSize, FontStyle.Bold))
                        {
                            // Set up text formatting with centered alignment
                            StringFormat stringFormat = new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center,
                                // Ensure no character is trimmed
                                Trimming = StringTrimming.None,
                                // Ensure the full line is measured
                                FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoClip
                            };

                            // Create a rectangle for text drawing area with vertical adjustment
                            // Add some extra padding at the top to prevent clipping
                            Rectangle textArea = new Rectangle(
                                HorizontalPadding, 
                                verticalOffset, // Apply the vertical offset here
                                ImageSize - (2 * HorizontalPadding), 
                                ImageSize - (verticalOffset) // Adjust height to maintain proportion
                            );

                            // Draw the text
                            using (Brush textBrush = new SolidBrush(Color.White))
                            {
                                graphics.DrawString(content, font, textBrush, textArea, stringFormat);
                            }
                        }

                        // Make sure the directory exists
                        string directory = Path.GetDirectoryName(savePath);
                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        // Save the bitmap as a PNG file
                        bitmap.Save(savePath, ImageFormat.Png);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // In a real application, you would log this exception
                Console.WriteLine($"Error generating image: {ex.Message}");
                return false;
            }
        }
    }
} 