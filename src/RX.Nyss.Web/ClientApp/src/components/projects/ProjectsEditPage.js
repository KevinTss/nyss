import React, { useState, Fragment, useEffect } from 'react';
import { connect } from "react-redux";
import { useLayout } from '../../utils/layout';
import { validators, createForm } from '../../utils/forms';
import * as projectsActions from './logic/projectsActions';
import Layout from '../layout/Layout';
import Form from '../forms/form/Form';
import FormActions from '../forms/formActions/FormActions';
import SubmitButton from '../forms/submitButton/SubmitButton';
import Typography from '@material-ui/core/Typography';
import TextInputField from '../forms/TextInputField';
import Button from "@material-ui/core/Button";
import { useMount } from '../../utils/lifecycle';
import { strings, stringKeys } from '../../strings';
import Grid from '@material-ui/core/Grid';
import AddIcon from '@material-ui/icons/Add';
import { MultiSelect } from '../forms/MultiSelect';
import { ProjectsHealthRiskItem } from './ProjectHealthRiskItem';
import { ProjectEmailNotificationItem } from './ProjectEmailNotificationItem';
import { ProjectSmsNotificationItem } from './ProjectSmsNotificationItem';
import { getSaveFormModel } from './logic/projectsService';
import { Loading } from '../common/loading/Loading';
import SelectField from '../forms/SelectField';
import MenuItem from "@material-ui/core/MenuItem";
import { ValidationMessage } from '../forms/ValidationMessage';

const ProjectsEditPageComponent = (props) => {
  const [healthRiskDataSource, setHealthRiskDataSource] = useState([]);
  const [selectedHealthRisks, setSelectedHealthRisks] = useState([]);
  const [emailNotifications, setEmailNotifications] = useState([]);
  const [smsNotifications, setSmsNotifications] = useState([]);

  useMount(() => {
    props.openEdition(props.nationalSocietyId, props.projectId);
  })

  useEffect(() => {
    setHealthRiskDataSource(props.healthRisks.map(hr => ({ label: hr.healthRiskName, value: hr.healthRiskId, data: hr })));
  }, [props.healthRisks])

  const [form, setForm] = useState(null);

  useEffect(() => {
    if (!props.data) {
      return;
    }

    let fields = {
      name: props.data.name,
      timeZoneId: props.data.timeZoneId
    };

    let validation = {
      name: [validators.required, validators.minLength(1), validators.maxLength(100)],
      timeZoneId: [validators.required, validators.minLength(1), validators.maxLength(50)]
    };

    setForm(createForm(fields, validation));
    setSelectedHealthRisks(props.data.projectHealthRisks);
    setEmailNotifications(props.data.emailAlertRecipients.map(ear => ({ id: ear.id, email: ear.email, key: ear.id })));
    setSmsNotifications(props.data.smsAlertRecipients.map(sar => ({ id: sar.id, phoneNumber: sar.phoneNumber, key: sar.id })));

    return () => setForm(null);
  }, [props.data]);

  const handleSubmit = (e) => {
    e.preventDefault();

    if (selectedHealthRisks.length === 0) {
      return;
    }

    if (!form.isValid()) {
      return;
    };

    props.edit(props.nationalSocietyId, props.projectId, getSaveFormModel(form.getValues(), selectedHealthRisks, emailNotifications, smsNotifications));
  };

  const onHealthRiskChange = (value, eventData) => {
    if (eventData.action === "select-option") {
      setSelectedHealthRisks([...selectedHealthRisks, eventData.option.data]);
    } else if (eventData.action === "remove-value" || eventData.action === "pop-value") {
      setSelectedHealthRisks(selectedHealthRisks.filter(hr => hr.healthRiskId !== eventData.removedValue.value));
    } else if (eventData.action === "clear") {
      setSelectedHealthRisks([]);
    }
  }

  const onEmailNotificationAdd = () => {
    setEmailNotifications([
      ...emailNotifications,
      {
        key: new Date().getTime(),
        id: null,
        email: ""
      }
    ])
  }

  const onEmailNotificationRemove = (key) =>
    setEmailNotifications(emailNotifications.filter(e => e.key !== key));

  const onSmsNotificationAdd = () => {
    setSmsNotifications([
      ...smsNotifications,
      {
        key: new Date().getTime(),
        id: null,
        phoneNumber: ""
      }
    ])
  }

  const onSmsNotificationRemove = (key) =>
    setSmsNotifications(smsNotifications.filter(e => e.key !== key));

  if (props.isFetching || !form) {
    return <Loading />;
  }

  return (
    <Fragment>
      {props.error && <ValidationMessage message={props.error} />}

      <Form onSubmit={handleSubmit} fullWidth style={{ maxWidth: 800 }}>
        <Grid container spacing={3}>
          <Grid item xs={12} sm={9}>
            <TextInputField
              label={strings(stringKeys.project.form.name)}
              name="name"
              field={form.fields.name}
            />
          </Grid>

          <Grid item xs={12}>
            <SelectField
              label={strings(stringKeys.project.form.timeZone)}
              field={form.fields.timeZoneId}
              name="timeZoneId"
            >
              {props.timeZones.map(timeZone => (
                <MenuItem key={timeZone.id} value={timeZone.id}>
                  {timeZone.displayName}
                </MenuItem>
              ))}
            </SelectField>
          </Grid>

          <Grid item xs={12}>
            <MultiSelect
              label={strings(stringKeys.project.form.healthRisks)}
              options={healthRiskDataSource}
              defaultValue={healthRiskDataSource.filter(hr => (selectedHealthRisks.some(shr => shr.healthRiskId === hr.value)))}
              onChange={onHealthRiskChange}
              error={selectedHealthRisks.length === 0 ? `${strings(stringKeys.validation.fieldRequired)}` : null}
            />
          </Grid>

          {selectedHealthRisks.length > 0 &&
            <Grid item xs={12}>
              <Typography variant="h3">{strings(stringKeys.project.form.healthRisksSetion)}</Typography>
            </Grid>
          }

          {selectedHealthRisks.map(selectedHealthRisk => (
            <ProjectsHealthRiskItem
              key={`projectsHealthRiskItem_${selectedHealthRisk.healthRiskId}`}
              form={form}
              projectHealthRisk={{ id: selectedHealthRisk.id }}
              healthRisk={selectedHealthRisk}
            />
          ))}

          <Grid item xs={12} sm={9}>
            <Typography variant="h3">{strings(stringKeys.project.form.emailNotificationsSection)}</Typography>
            <Typography variant="subtitle1">{strings(stringKeys.project.form.emailNotificationDescription)}</Typography>

            {emailNotifications.map(emailNotification => (
              <ProjectEmailNotificationItem
                key={`projectEmailNotificationItem_${emailNotification.key}`}
                itemKey={emailNotification.key}
                form={form}
                emailNotification={emailNotification}
                onRemove={() => onEmailNotificationRemove(emailNotification.key)}
              />
            ))}
          </Grid>
          <Grid item xs={12} sm={9}>
            <Button startIcon={<AddIcon />} onClick={onEmailNotificationAdd}>{strings(stringKeys.project.form.addEmail)}</Button>
          </Grid>

          <Grid item xs={12} sm={9}>
            <Typography variant="h3">{strings(stringKeys.project.form.smsNotificationsSetion)}</Typography>
            <Typography variant="subtitle1">{strings(stringKeys.project.form.smsNotificationDescription)}</Typography>

            {smsNotifications.map(smsNotification => (
              <ProjectSmsNotificationItem
                key={`projectSmsNotificationItem_${smsNotification.key}`}
                itemKey={smsNotification.key}
                form={form}
                smsNotification={smsNotification}
                onRemove={() => onSmsNotificationRemove(smsNotification.key)}
              />
            ))}
          </Grid>
          <Grid item xs={12} sm={9}>
            <Button startIcon={<AddIcon />} onClick={onSmsNotificationAdd}>{strings(stringKeys.project.form.addSms)}</Button>
          </Grid>
        </Grid>

        <FormActions>
          <Button onClick={() => props.goToOverview(props.nationalSocietyId, props.projectId)}>{strings(stringKeys.form.cancel)}</Button>
          <SubmitButton isFetching={props.isSaving}>{strings(stringKeys.project.form.update)}</SubmitButton>
        </FormActions>
      </Form>
    </Fragment>
  );
}

ProjectsEditPageComponent.propTypes = {
};

const mapStateToProps = (state, ownProps) => ({
  healthRisks: state.projects.formHealthRisks,
  timeZones: state.projects.formTimeZones,
  projectId: ownProps.match.params.projectId,
  nationalSocietyId: ownProps.match.params.nationalSocietyId,
  isFetching: state.projects.formFetching,
  isSaving: state.projects.formSaving,
  data: state.projects.formData,
  error: state.projects.formError
});

const mapDispatchToProps = {
  openEdition: projectsActions.openEdition.invoke,
  edit: projectsActions.edit.invoke,
  goToOverview: projectsActions.goToOverview
};

export const ProjectsEditPage = useLayout(
  Layout,
  connect(mapStateToProps, mapDispatchToProps)(ProjectsEditPageComponent)
);
