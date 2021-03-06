import React from 'react';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import CardHeader from '@material-ui/core/CardHeader';
import { strings, stringKeys } from "../../../strings";
import { ReportsMap } from "../../maps/ReportsMap";

export const ProjectsDashboardReportsMap = ({ data, details, detailsFetching, projectId, getReportHealthRisks }) => {
  const handleMarkerClick = (lat, lng) =>
    getReportHealthRisks(projectId, lat, lng);

  return (
    <Card data-printable={true}>
      <CardHeader title={strings(stringKeys.project.dashboard.map.title)} />
      <CardContent>
        <ReportsMap
          data={data}
          details={details}
          detailsFetching={detailsFetching}
          onMarkerClick={handleMarkerClick}
        />
      </CardContent>
    </Card>
  );
}
