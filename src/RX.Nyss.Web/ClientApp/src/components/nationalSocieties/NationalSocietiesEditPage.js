import React, { useEffect, useState, Fragment } from 'react';
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { useLayout } from '../../utils/layout';
import { validators, createForm } from '../../utils/forms';
import * as nationalSocietiesActions from './logic/nationalSocietiesActions';
import Layout from '../layout/Layout';
import Form from '../forms/form/Form';
import FormActions from '../forms/formActions/FormActions';
import SubmitButton from '../forms/submitButton/SubmitButton';
import TextInputField from '../forms/TextInputField';
import SelectField from '../forms/SelectField';
import MenuItem from "@material-ui/core/MenuItem";
import Button from "@material-ui/core/Button";
import { Loading } from '../common/loading/Loading';
import { useMount } from '../../utils/lifecycle';
import Grid from '@material-ui/core/Grid';
import { strings, stringKeys } from '../../strings';
import { ValidationMessage } from '../forms/ValidationMessage';

const NationalSocietiesEditPageComponent = (props) => {
  const [form, setForm] = useState(null);

  useMount(() => {
    props.openEdition(props.match);
  });

  useEffect(() => {
    if (!props.data) {
      return;
    }

    const fields = {
      id: props.data.id,
      name: props.data.name,
      contentLanguageId: props.data.contentLanguageId.toString(),
      countryId: props.data.countryId.toString()
    };

    const validation = {
      id: [validators.required],
      name: [validators.required, validators.minLength(3)],
      contentLanguageId: [validators.required],
      countryId: [validators.required]
    };

    setForm(createForm(fields, validation));
  }, [props.data, props.match]);

  const handleSubmit = (e) => {
    e.preventDefault();

    if (!form.isValid()) {
      return;
    };

    const values = form.getValues();
    props.edit({
      id: props.data.id,
      name: values.name,
      contentLanguageId: parseInt(values.contentLanguageId),
      countryId: parseInt(values.countryId)
    });
  };

  if (props.isFetching || !form) {
    return <Loading />;
  }

  return (
    <Fragment>
      {props.error && <ValidationMessage message={props.error} />}

      <Form onSubmit={handleSubmit}>
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <TextInputField
              label={strings(stringKeys.nationalSociety.form.name)}
              name="name"
              field={form.fields.name}
              autoFocus
            />
          </Grid>

          <Grid item xs={12}>
            <SelectField
              label={strings(stringKeys.nationalSociety.form.country)}
              name="country"
              field={form.fields.countryId}
            >
              {props.countries.map(country => (
                <MenuItem key={`country${country.id}`} value={country.id.toString()}>{country.name}</MenuItem>
              ))}
            </SelectField>
          </Grid>

          <Grid item xs={12}>
            <SelectField
              label={strings(stringKeys.nationalSociety.form.contentLanguage)}
              name="contentLanguage"
              field={form.fields.contentLanguageId}
            >
              {props.contentLanguages.map(language => (
                <MenuItem key={`contentLanguage${language.id}`} value={language.id.toString()}>{language.name}</MenuItem>
              ))}
            </SelectField>
          </Grid>
        </Grid>

        <FormActions>
          <Button onClick={() => props.goToOverview(props.data.id)}>
            {strings(stringKeys.form.cancel)}
          </Button>

          <SubmitButton isFetching={props.isSaving}>
            {strings(stringKeys.nationalSociety.form.update)}
          </SubmitButton>
        </FormActions>
      </Form>
    </Fragment>
  );
}

NationalSocietiesEditPageComponent.propTypes = {
  getNationalSocieties: PropTypes.func,
  list: PropTypes.array
};

const mapStateToProps = state => ({
  contentLanguages: state.appData.contentLanguages,
  countries: state.appData.countries,
  error: state.nationalSocieties.formError,
  isFetching: state.nationalSocieties.formFetching,
  isSaving: state.nationalSocieties.formSaving,
  data: state.nationalSocieties.formData
});

const mapDispatchToProps = {
  openEdition: nationalSocietiesActions.openEdition.invoke,
  edit: nationalSocietiesActions.edit.invoke,
  goToOverview: nationalSocietiesActions.goToOverview
};

export const NationalSocietiesEditPage = useLayout(
  Layout,
  connect(mapStateToProps, mapDispatchToProps)(NationalSocietiesEditPageComponent)
);
