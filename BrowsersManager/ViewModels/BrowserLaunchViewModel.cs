using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using BrowsersManager.Models;
using BrowsersManager.Services;

namespace BrowsersManager.ViewModels
{
    public class BrowserLaunchViewModel
    {
        private readonly BrowserProcessService _processService;
        private bool _isLaunching;

        public ObservableCollection<BrowserGroup> BrowserGroups { get; }

        public bool IsLaunching
        {
            get => _isLaunching;
            private set
            {
                _isLaunching = value;
            }
        }

        public BrowserLaunchViewModel()
        {
            _processService = new BrowserProcessService();
            BrowserGroups = new ObservableCollection<BrowserGroup>();
            LoadBrowserGroups();
        }

        private void LoadBrowserGroups()
        {
            try
            {
                var groups = BrowserGroup.LoadAllGroups();
                BrowserGroups.Clear();
                foreach (var group in groups.Values)
                {
                    BrowserGroups.Add(group);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading browser groups: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Launches all browsers in a group or selected profiles within the group
        /// </summary>
        /// <param name="group">Browser group to launch</param>
        /// <param name="url">Optional URL to open in all browsers</param>
        /// <param name="selectedProfiles">Optional list of specific profiles to launch</param>
        /// <returns>Task representing the operation</returns>
        public async Task LaunchGroupAsync(BrowserGroup group, string url = null, string[] selectedProfiles = null)
        {
            if (group == null) return;

            try
            {
                IsLaunching = true;
                
                // Now we don't need to find BrowserType as it's handled in BrowserProcessService
                var launchedCount = await _processService.LaunchBrowserGroupAsync(group, url, selectedProfiles);
                MessageBox.Show($"成功启动 {launchedCount} 个浏览器，来自组 {group.Description}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动浏览器组时出错: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                IsLaunching = false;
            }
        }

        /// <summary>
        /// Launches all browsers in a group by group name
        /// </summary>
        /// <param name="groupName">Name of the browser group to launch</param>
        /// <param name="url">Optional URL to open in all browsers</param>
        /// <param name="selectedProfiles">Optional list of specific profiles to launch</param>
        /// <returns>Task representing the operation</returns>
        public async Task LaunchGroupByNameAsync(string groupName, string url = null, string[] selectedProfiles = null)
        {
            try
            {
                IsLaunching = true;
                
                var launchedCount = await _processService.LaunchBrowserGroupAsync(groupName, url, selectedProfiles);
                MessageBox.Show($"成功启动 {launchedCount} 个浏览器，来自组 {groupName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动浏览器组时出错: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                IsLaunching = false;
            }
        }
    }
} 