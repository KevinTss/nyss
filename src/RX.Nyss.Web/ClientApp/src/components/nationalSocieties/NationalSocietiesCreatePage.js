import React, { useState, Fragment } from 'react';
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { useLayout } from '../../utils/layout';
import { validators, createForm } from '../../utils/forms';
import * as nationalSocietiesActions from './logic/nationalSocietiesActions';
import * as appActions from '../app/logic/appActions';
import Layout from '../layout/Layout';
import Form from '../forms/form/Form';
import FormActions from '../forms/formActions/FormActions';
import SubmitButton from '../forms/submitButton/SubmitButton';
import TextInputField from '../forms/TextInputField';
import SelectField from '../forms/SelectField';
import MenuItem from "@material-ui/core/MenuItem";
import Button from "@material-ui/core/Button";
import { useMount } from '../../utils/lifecycle';
import Grid from '@material-ui/core/Grid';
import { strings, stringKeys } from '../../strings';
import { ValidationMessage } from '../forms/ValidationMessage';

const NationalSocietiesCreatePageComponent = (props) => {
  const [form] = useState(() => {
    const fields = {
      name: "",
      contentLanguageId: "",
      countryId: ""
    };

    const validation = {
      name: [validators.required, validators.minLength(1)],
      contentLanguageId: [validators.required],
      countryId: [validators.required]
    };

    return createForm(fields, validation);
  });

  useMount(() => {
    props.openModule(props.match.path, props.match.params)
  })

  const handleSubmit = (e) => {
    e.preventDefault();

    if (!form.isValid()) {
      return;
    };

    const values = form.getValues();
    props.create({
      name: values.name,
      contentLanguageId: parseInt(values.contentLanguageId),
      countryId: parseInt(values.countryId)
    });
  };

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
          <Button onClick={() => props.goToList()}>
            {strings(stringKeys.form.cancel)}
          </Button>

          <SubmitButton isFetching={props.isSaving}>
            {strings(stringKeys.nationalSociety.form.create)}
          </SubmitButton>
        </FormActions>
      </Form>
    </Fragment>
  );
}

NationalSocietiesCreatePageComponent.propTypes = {
  getNationalSocieties: PropTypes.func,
  openModule: PropTypes.func,
  list: PropTypes.array
};

const mapStateToProps = state => ({
  contentLanguages: state.appData.contentLanguages,
  countries: state.appData.countries,
  error: state.nationalSocieties.formError,
  isSaving: state.nationalSocieties.formSaving
});

const mapDispatchToProps = {
  getList: nationalSocietiesActions.getList.invoke,
  create: nationalSocietiesActions.create.invoke,
  goToList: nationalSocietiesActions.goToList,
  openModule: appActions.openModule.invoke
};

export const NationalSocietiesCreatePage = useLayout(
  Layout,
  connect(mapStateToProps, mapDispatchToProps)(NationalSocietiesCreatePageComponent)
);
