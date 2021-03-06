import styles from "./ReportsEditPage.module.scss";

import React, { useEffect, useState, Fragment } from 'react';
import { connect } from "react-redux";
import { useLayout } from '../../utils/layout';
import { validators, createForm } from '../../utils/forms';
import * as reportsActions from './logic/reportsActions';
import Layout from '../layout/Layout';
import Form from '../forms/form/Form';
import FormActions from '../forms/formActions/FormActions';
import SubmitButton from '../forms/submitButton/SubmitButton';
import TextInputField from '../forms/TextInputField';
import DateInputField from "../forms/DateInputField";
import SelectField from '../forms/SelectField';
import MenuItem from "@material-ui/core/MenuItem";
import Button from "@material-ui/core/Button";
import { Loading } from '../common/loading/Loading';
import { useMount } from '../../utils/lifecycle';
import { strings, stringKeys } from '../../strings';
import Grid from '@material-ui/core/Grid';
import { ValidationMessage } from '../forms/ValidationMessage';
import dayjs from "dayjs";

const ReportsEditPageComponent = (props) => {
    const [form, setForm] = useState(null);

    useMount(() => {
        props.openEdition(props.projectId, props.reportId);
    });

    useEffect(() => {
        if (!props.data) {
            return;
        }

        const fields = {
            id: props.data.id,
            date: dayjs(props.data.date),
            healthRiskId: props.data.healthRiskId.toString(),
            countMalesBelowFive: props.data.countMalesBelowFive.toString(),
            countMalesAtLeastFive: props.data.countMalesAtLeastFive.toString(),
            countFemalesBelowFive: props.data.countFemalesBelowFive.toString(),
            countFemalesAtLeastFive: props.data.countFemalesAtLeastFive.toString(),
            referredCount: props.data.referredCount.toString(),
            deathCount: props.data.deathCount.toString(),
            fromOtherVillagesCount: props.data.fromOtherVillagesCount.toString()
        };

        const validation = {
            date: [validators.required],
            healthRiskId: [validators.required],
            countMalesBelowFive: [validators.required, validators.integer, validators.nonNegativeNumber],
            countMalesAtLeastFive: [validators.required, validators.integer, validators.nonNegativeNumber],
            countFemalesBelowFive: [validators.required, validators.integer, validators.nonNegativeNumber],
            countFemalesAtLeastFive: [validators.required, validators.integer, validators.nonNegativeNumber],
            referredCount: [validators.integer, validators.nonNegativeNumber],
            deathCount: [validators.integer, validators.nonNegativeNumber],
            fromOtherVillagesCount: [validators.integer, validators.nonNegativeNumber]
        };

        setForm(createForm(fields, validation));
    }, [props.data, props.match]);

    const handleSubmit = (e) => {
        e.preventDefault();

        if (!form.isValid()) {
            return;
        };

        const values = form.getValues();
        props.edit(props.projectId, props.reportId, {
            date: values.date.format('YYYY-MM-DDTHH:mm:ss'),
            healthRiskId: parseInt(values.healthRiskId),
            countMalesBelowFive: parseInt(values.countMalesBelowFive),
            countMalesAtLeastFive: parseInt(values.countMalesAtLeastFive),
            countFemalesBelowFive: parseInt(values.countFemalesBelowFive),
            countFemalesAtLeastFive: parseInt(values.countFemalesAtLeastFive),
            referredCount: values.referredCount === "" ? null : parseInt(values.referredCount),
            deathCount: values.deathCount === "" ? null : parseInt(values.deathCount),
            fromOtherVillagesCount: values.fromOtherVillagesCount === "" ? null : parseInt(values.fromOtherVillagesCount)
        });
    };

    if (props.isFetching || !form || !props.data) {
        return <Loading />;
    }

    return (
        <Fragment>
            {props.error && <ValidationMessage message={props.error} />}
            <Form onSubmit={handleSubmit}>
                <Grid container spacing={3}>
                    <Grid item xs={12}>
                        <DateInputField
                            className={styles.fullWidth}
                            label={strings(stringKeys.reports.form.date)}
                            name="date"
                            field={form.fields.date}
                        />
                    </Grid>
                </Grid>
                <Grid container spacing={3}>
                    <Grid item xs={12}>
                        <SelectField
                            label={strings(stringKeys.reports.form.healthRisk)}
                            name="healthRiskId"
                            field={form.fields.healthRiskId}
                        >
                            {props.healthRisks.map(healthRisk => (
                                <MenuItem key={`healthRisk_${healthRisk.id}`} value={healthRisk.id.toString()}>
                                    {healthRisk.name}
                                </MenuItem>
                            ))}
                        </SelectField>
                    </Grid>
                </Grid>

                <Grid container spacing={3}>
                    <Grid item xs={12}>
                        <TextInputField
                            label={strings(stringKeys.reports.form.malesBelowFive)}
                            name="countMalesBelowFive"
                            field={form.fields.countMalesBelowFive}
                        />
                    </Grid>
                </Grid>

                <Grid container spacing={3}>
                    <Grid item xs={12}>
                        <TextInputField
                            label={strings(stringKeys.reports.form.malesAtLeastFive)}
                            name="countMalesAtLeastFive"
                            field={form.fields.countMalesAtLeastFive}
                        />
                    </Grid>
                </Grid>

                <Grid container spacing={3}>
                    <Grid item xs={12}>
                        <TextInputField
                            label={strings(stringKeys.reports.form.femalesBelowFive)}
                            name="countFemalesBelowFive"
                            field={form.fields.countFemalesBelowFive}
                        />
                    </Grid>
                </Grid>

                <Grid container spacing={3}>
                    <Grid item xs={12}>
                        <TextInputField
                            label={strings(stringKeys.reports.form.femalesAtLeastFive)}
                            name="countFemalesAtLeastFive"
                            field={form.fields.countFemalesAtLeastFive}
                        />
                    </Grid>
                </Grid>

                <Grid container spacing={3}>
                    <Grid item xs={12}>
                        <TextInputField
                            label={strings(stringKeys.reports.form.referredCount)}
                            name="referredCount"
                            field={form.fields.referredCount}
                            disabled={props.data.reportType !== "DataCollectionPoint"}
                        />
                    </Grid>
                </Grid>

                <Grid container spacing={3}>
                    <Grid item xs={12}>
                        <TextInputField
                            label={strings(stringKeys.reports.form.deathCount)}
                            name="deathCount"
                            field={form.fields.deathCount}
                            disabled={props.data.reportType !== "DataCollectionPoint"}
                        />
                    </Grid>
                </Grid>

                <Grid container spacing={3}>
                    <Grid item xs={12}>
                        <TextInputField
                            label={strings(stringKeys.reports.form.fromOtherVillagesCount)}
                            name="fromOtherVillagesCount"
                            field={form.fields.fromOtherVillagesCount}
                            disabled={props.data.reportType !== "DataCollectionPoint"}
                        />
                    </Grid>
                </Grid>

                <FormActions>
                    <Button onClick={() => props.goToList(props.projectId)}>{strings(stringKeys.form.cancel)}</Button>
                    <SubmitButton isFetching={props.isSaving}>{strings(stringKeys.reports.form.update)}</SubmitButton>
                </FormActions>
            </Form>
        </Fragment>
    );
}

ReportsEditPageComponent.propTypes = {
};

const mapStateToProps = (state, ownProps) => ({
    reportId: ownProps.match.params.reportId,
    projectId: ownProps.match.params.projectId,
    isFetching: state.reports.formFetching,
    isSaving: state.reports.formSaving,
    data: state.reports.formData,
    error: state.reports.formError,
    healthRisks: state.reports.formHealthRisks
});

const mapDispatchToProps = {
    openEdition: reportsActions.openEdition.invoke,
    edit: reportsActions.edit.invoke,
    goToList: reportsActions.goToList
};

export const ReportsEditPage = useLayout(
    Layout,
    connect(mapStateToProps, mapDispatchToProps)(ReportsEditPageComponent)
);
