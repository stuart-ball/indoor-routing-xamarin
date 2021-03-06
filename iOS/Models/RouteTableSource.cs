﻿// Copyright 2020 Esri.

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.OpenSourceApps.IndoorRouting.iOS.Helpers;
using Foundation;
using UIKit;

namespace Esri.ArcGISRuntime.OpenSourceApps.IndoorRouting.iOS.Models
{
    /// <summary>
    /// Table data source for the route stops view
    /// </summary>
    public class RouteTableSource : UITableViewSource
    {
        // Identifies for start and end cell, since they have different appearances
        private const string StartCellIdentifier = "startCellID";
        private const string EndCellIdentifier = "endCellID";

        /// <summary>
        /// The route stops; currently only supports one origin and one destination.
        /// </summary>
        private readonly IEnumerable<Feature> _items;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Esri.ArcGISRuntime.OpenSourceApps.IndoorRouting.iOS.Models.RouteTableSource"/> class.
        /// </summary>
        /// <param name="items">Table Items.</param>
        internal RouteTableSource(List<Feature> items) => _items = items;

        /// <summary>
        /// Called by the TableView to determine how many cells to create for that particular section.
        /// </summary>
        /// <returns>The rows in section.</returns>
        /// <param name="tableview">Containing Tableview.</param>
        /// <param name="section">Specific Section.</param>
        public override nint RowsInSection(UITableView tableview, nint section) => _items?.Count() ?? 0;

        /// <summary>
        /// Called by the TableView to get the actual UITableViewCell to render for the particular row
        /// </summary>
        /// <returns>The cell.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            // Used to create the 2 route card cells
            var cellIdentifier = indexPath.Row == 0 ? StartCellIdentifier : EndCellIdentifier;
            var cell = tableView.DequeueReusableCell(cellIdentifier);

            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Subtitle, cellIdentifier);
                string imageName = indexPath.Row == 0 ? "StartCircle" : "EndCircle";
                cell.ImageView.Image = UIImage.FromBundle(imageName);
                cell.BackgroundColor = tableView.BackgroundColor;
            }

            try
            {
                if (_items.ElementAt(indexPath.Row) != null)
                {
                    var item = _items.ElementAt(indexPath.Row);
                    cell.TextLabel.Text = item.Attributes[AppSettings.CurrentSettings.LocatorFields[0]].ToString();
                    cell.DetailTextLabel.Text = $"{"FloorLabel".Localize()} {item.Attributes[AppSettings.CurrentSettings.RoomsLayerFloorColumnName]}";

                    return cell;
                }
                // Null entry is a stand-in for using the device's current location
                else if (AppSettings.CurrentSettings.IsLocationServicesEnabled)
                {
                    cell.TextLabel.Text = AppSettings.LocalizedCurrentLocationString;
                    cell.DetailTextLabel.Text = string.Empty;
                    return cell;
                }
                else
                {
                    cell.TextLabel.Text = "UnknownLocationLabel".Localize();
                    cell.DetailTextLabel.Text = string.Empty;
                    return cell;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Instance.LogException(ex);
                throw;
            }
        }
    } 
}