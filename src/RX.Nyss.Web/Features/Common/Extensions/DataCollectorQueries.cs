﻿using System;
using System.Linq;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Data.Models;
using RX.Nyss.Web.Features.Common.Dto;

namespace RX.Nyss.Web.Features.Common.Extensions
{
    public static class DataCollectorQueries
    {
        public static IQueryable<DataCollector> FilterByNationalSociety(this IQueryable<DataCollector> dataCollectors, int nationalSocietyId) =>
            dataCollectors.Where(dc => dc.Project.NationalSocietyId == nationalSocietyId);

        public static IQueryable<DataCollector> FilterByDataCollectorType(this IQueryable<DataCollector> dataCollectors, DataCollectorType? dataCollectorType) =>
            dataCollectorType switch
            {
                DataCollectorType.Human =>
                dataCollectors.Where(dc => dc.DataCollectorType == DataCollectorType.Human),

                DataCollectorType.CollectionPoint =>
                dataCollectors.Where(dc => dc.DataCollectorType == DataCollectorType.CollectionPoint),

                _ =>
                dataCollectors
            };

        public static IQueryable<DataCollector> FilterByType(this IQueryable<DataCollector> dataCollectors, DataCollectorType? dataCollectorType) =>
            dataCollectorType switch
            {
                DataCollectorType.Human =>
                dataCollectors.Where(dc => dc.DataCollectorType == DataCollectorType.Human),

                DataCollectorType.CollectionPoint =>
                dataCollectors.Where(dc => dc.DataCollectorType == DataCollectorType.CollectionPoint),

                _ =>
                dataCollectors
            };

        public static IQueryable<DataCollector> FilterByArea(this IQueryable<DataCollector> dataCollectors, AreaDto area) =>
            area?.Type switch
            {
                AreaType.Region =>
                dataCollectors.Where(dc => dc.Village.District.Region.Id == area.Id),

                AreaType.District =>
                dataCollectors.Where(dc => dc.Village.District.Id == area.Id),

                AreaType.Village =>
                dataCollectors.Where(dc => dc.Village.Id == area.Id),

                AreaType.Zone =>
                dataCollectors.Where(dc => dc.Zone.Id == area.Id),

                _ =>
                dataCollectors
            };

        public static IQueryable<DataCollector> FilterByArea(this IQueryable<DataCollector> dataCollectors, Area area) =>
            area?.AreaType switch
            {
                AreaType.Region =>
                dataCollectors.Where(dc => dc.Village.District.Region.Id == area.AreaId),

                AreaType.District =>
                dataCollectors.Where(dc => dc.Village.District.Id == area.AreaId),

                AreaType.Village =>
                dataCollectors.Where(dc => dc.Village.Id == area.AreaId),

                AreaType.Zone =>
                dataCollectors.Where(dc => dc.Zone.Id == area.AreaId),

                _ =>
                dataCollectors
            };

        public static IQueryable<DataCollector> FilterByTrainingMode(this IQueryable<DataCollector> dataCollectors, bool isInTraining) =>
            dataCollectors.Where(dc => isInTraining
                ? dc.IsInTrainingMode
                : !dc.IsInTrainingMode);

        public static IQueryable<DataCollector> FilterByProject(this IQueryable<DataCollector> dataCollectors, int projectId) =>
            dataCollectors.Where(dc => dc.Project.Id == projectId);

        public static IQueryable<DataCollector> FilterOnlyNotDeletedBefore(this IQueryable<DataCollector> dataCollectors, DateTime startDate) =>
            dataCollectors.Where(dc => dc.DeletedAt == null || dc.DeletedAt > startDate);
    }
}
