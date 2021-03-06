import styles from "./AlertsAssessment.module.scss";
import React, { useEffect, Fragment } from 'react';
import { connect } from "react-redux";
import { useLayout } from '../../utils/layout';
import * as alertsActions from './logic/alertsActions';
import Layout from '../layout/Layout';
import { Loading } from '../common/loading/Loading';
import { useMount } from '../../utils/lifecycle';
import Grid from '@material-ui/core/Grid';
import { stringKeys, strings } from '../../strings';
import DisplayField from "../forms/DisplayField";
import { AlertsAssessmentReport } from "./components/AlertsAssessmentReport";
import { assessmentStatus } from "./logic/alertsConstants";
import Divider from "@material-ui/core/Divider";
import { AlertsAssessmentActions } from "./components/AlertsAssessmentActions";

const getAssessmentStatusInformation = (status) => {
  switch (status) {
    case assessmentStatus.open:
    case assessmentStatus.toEscalate:
    case assessmentStatus.toDismiss:
      return stringKeys.alerts.assess.introduction;

    case assessmentStatus.closed:
      return stringKeys.alerts.assess.statusDescription.closed;
    case assessmentStatus.escalated:
      return stringKeys.alerts.assess.statusDescription.escalated;
    case assessmentStatus.dismissed:
      return stringKeys.alerts.assess.statusDescription.dismissed;
    case assessmentStatus.rejected:
      return stringKeys.alerts.assess.statusDescription.rejected;
    default:
      throw new Error("Wrong status")
  }
}

const AlertsAssessmentPageComponent = ({ alertId, projectId, data, ...props }) => {
  useMount(() => {
    props.openAssessment(projectId, alertId);
  });

  useEffect(() => {
    if (!props.data) {
      return;
    }

  }, [props.data, props.match]);

  if (props.isFetching || !data) {
    return <Loading />;
  }

  return (
    <Fragment>
      <div className={styles.form}>
        <div className={styles.introduction}>
          {strings(getAssessmentStatusInformation(data.assessmentStatus))}
        </div>

        {data.assessmentStatus === assessmentStatus.closed && data.comments && (
          <DisplayField
            label={strings(stringKeys.alerts.assess.comments)}
            value={data.comments}
          />
        )}

        <DisplayField
          label={strings(stringKeys.alerts.assess.caseDefinition)}
          value={data.caseDefinition}
        />

        <Divider />

        <div className={styles.reportsTitle}>{strings(stringKeys.alerts.assess.reports)}</div>

        <Grid container spacing={3}>
          {data.reports.map(report => (
            <Grid item xs={12} key={`report_${report.id}`}>
              <AlertsAssessmentReport
                report={report}
                alertId={alertId}
                assessmentStatus={data.assessmentStatus}
                acceptReport={props.acceptReport}
                dismissReport={props.dismissReport}
                projectIsClosed={props.projectIsClosed}
              />
            </Grid>
          ))}
        </Grid>

        <AlertsAssessmentActions
          alertId={alertId}
          projectId={projectId}
          alertAssessmentStatus={data.assessmentStatus}

          goToList={props.goToList}

          closeAlert={props.closeAlert}
          isClosing={props.isClosing}

          escalateAlert={props.escalateAlert}
          isEscalating={props.isEscalating}

          dismissAlert={props.dismissAlert}
          isDismissing={props.isDismissing}

          notificationEmails={data.notificationEmails}
          notificationPhoneNumbers={data.notificationPhoneNumbers}
        />
      </div>
    </Fragment>
  );
}

AlertsAssessmentPageComponent.propTypes = {
};

const mapStateToProps = (state, ownProps) => ({
  projectId: ownProps.match.params.projectId,
  alertId: ownProps.match.params.alertId,
  isFetching: state.alerts.formFetching,
  isSaving: state.alerts.formSaving,
  isEscalating: state.alerts.formEscalating,
  isClosing: state.alerts.formClosing,
  isDismissing: state.alerts.formDismissing,
  data: state.alerts.formData,
  projectIsClosed: state.appData.siteMap.parameters.projectIsClosed,
});

const mapDispatchToProps = {
  goToList: alertsActions.goToList,
  openAssessment: alertsActions.openAssessment.invoke,
  acceptReport: alertsActions.acceptReport.invoke,
  dismissReport: alertsActions.dismissReport.invoke,
  escalateAlert: alertsActions.escalateAlert.invoke,
  closeAlert: alertsActions.closeAlert.invoke,
  dismissAlert: alertsActions.dismissAlert.invoke,
};

export const AlertsAssessmentPage = useLayout(
  Layout,
  connect(mapStateToProps, mapDispatchToProps)(AlertsAssessmentPageComponent)
);
