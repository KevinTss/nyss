import React from 'react';
import { StringsEditor } from './components/common/stringsEditor/StringsEditor';

let stringList = {};

let showKeys = false;

export const areStringKeysDisplayed = () => showKeys;

export const stringKeys = {
  login: {
    title: "login.title",
    welcome: "login.welcome",
    signIn: "login.signIn",
    email: "login.email",
    password: "login.password",
    forgotPassword: "login.forgotPassword",
    notSucceeded: "login.notSucceeded",
    lockedOut: "login.lockedOut"
  },
  nationalSociety: {
    title: "nationalSociety.title",
    addNew: "nationalSociety.addNew",
    edit: "nationalSociety.edit",
    form: {
      creationTitle: "nationalSociety.form.creationTitle",
      editionTitle: "nationalSociety.form.editionTitle",
      name: "nationalSociety.form.name",
      country: "nationalSociety.form.country",
      contentLanguage: "nationalSociety.form.contentLanguage",
      create: "nationalSociety.form.create",
      update: "nationalSociety.form.update"
    },
    list: {
      name: "nationalSociety.list.name",
      country: "nationalSociety.list.country",
      startDate: "nationalSociety.list.startDate",
      headManager: "nationalSociety.list.headManager",
      technicalAdvisor: "nationalSociety.list.technicalAdvisor",
      removalConfirmation: "nationalSociety.list.removalConfirmation",
    },
    dashboard: {
      title: "nationalSociety.dashboard.title"
    },
    overview: {
      title: "nationalSociety.dashboard.overview"
    },
    setHead: {
      notAMemberOfSociety: "nationalSociety.setHead.notAMemberOfSociety"
    }
  },
  healthRisk: {
    title: "healthRisk.title",
    addNew: "healthRisk.addNew",
    form: {
      creationTitle: "healthRisk.form.creationTitle",
      editionTitle: "healthRisk.form.editionTitle",
      healthRiskCode: "healthRisk.form.healthRiskCode",
      healthRiskType: "healthRisk.form.healthRiskType",
      translationsSetion: "healthRisk.form.translationsSetion",
      alertsSetion: "healthRisk.form.alertsSetion",
      alertRuleDescription: "healthRisk.form.alertRuleDescription",
      alertRuleCountThreshold: "healthRisk.form.alertRuleCountThreshold",
      alertRuleDaysThreshold: "healthRisk.form.alertRuleDaysThreshold",
      alertRuleKilometersThreshold: "healthRisk.form.alertRuleKilometersThreshold",
      contentLanguageName: "healthRisk.form.contentLanguageName",
      contentLanguageCaseDefinition: "healthRisk.form.contentLanguageCaseDefinition",
      contentLanguageFeedbackMessage: "healthRisk.form.contentLanguageFeedbackMessage",
      create: "healthRisk.form.create",
      update: "healthRisk.form.update"
    },
    list: {
      healthRiskCode: "healthRisk.list.healthRiskCode",
      name: "healthRisk.list.name",
      healthRiskType: "healthRisk.list.healthRiskType",
      removalConfirmation: "healthRisk.list.removalConfirmation"
    },
    constants: {
      healthRiskType: {
        human: "healthRisk.type.human",
        nonhuman: "healthRisk.type.nonhuman",
        unusualevent: "healthRisk.type.unusualevent",
        activity: "healthRisk.type.activity",
      }
    }
  },
  smsGateway: {
    title: "smsGateway.title",
    addNew: "smsGateway.addNew",
    form: {
      creationTitle: "smsGateway.form.creationTitle",
      editionTitle: "smsGateway.form.editionTitle",
      name: "smsGateway.form.name",
      apiKey: "smsGateway.form.apiKey",
      gatewayType: "smsGateway.form.gatewayType",
      create: "smsGateway.form.create",
      update: "smsGateway.form.update"
    },
    list: {
      name: "smsGateway.list.name",
      apiKey: "smsGateway.list.apiKey",
      gatewayType: "smsGateway.list.gatewayType",
      removalConfirmation: "smsGateway.list.removalConfirmation"
    },
  },
  project: {
    title: "project.title",
    addNew: "project.addNew",
    form: {
      creationTitle: "project.form.creationTitle",
      editionTitle: "project.form.editionTitle",
      name: "project.form.name",
      timeZone: "project.form.timeZone",
      healthRisks: "project.form.healthRisks",
      healthRisksSetion: "project.form.healthRisksSetion",
      caseDefinition: "project.form.caseDefinition",
      feedbackMessage: "project.form.feedbackMessage",
      alertsSetion: "project.form.alertsSetion",
      alertRuleCountThreshold: "project.form.alertRuleCountThreshold",
      alertRuleDaysThreshold: "project.form.alertRuleDaysThreshold",
      alertRuleKilometersThreshold: "project.form.alertRuleKilometersThreshold",
      notificationsSetion: "project.form.notificationsSetion",
      notificationDescription: "project.form.notificationDescription",
      email: "project.form.email",
      addEmail: "project.form.addEmail",
      create: "project.form.create",
      update: "project.form.update"
    },
    list: {
      name: "project.list.name",
      totalReportCount: "project.list.totalReportCount",
      activeDataCollectorCount: "project.list.activeDataCollectorCount",
      startDate: "project.list.startDate",
      escalatedAlertCount: "project.list.escalatedAlertCount",
      supervisorCount: "project.list.supervisorCount",
      endDate: "project.list.endDate",
      ongoing: "project.list.ongoing",
      open: "project.list.open",
      edit: "project.list.edit",
      remove: "project.list.remove",
      removalConfirmation: "project.list.removalConfirmation"
    },
    dashboard: {
      title: "project.dashboard.title"
    },
    settings: "project.settings.title"
  },
  globalCoordinator: {
    title: "globalCoordinator.title",
    addNew: "globalCoordinator.addNew",
    form: {
      creationTitle: "globalCoordinator.form.creationTitle",
      editionTitle: "globalCoordinator.form.editionTitle",
      name: "globalCoordinator.form.name",
      email: "globalCoordinator.form.email",
      phoneNumber: "globalCoordinator.form.phoneNumber",
      additionalPhoneNumber: "globalCoordinator.form.additionalPhoneNumber",
      organization: "globalCoordinator.form.organization",
      create: "globalCoordinator.form.create",
      update: "globalCoordinator.form.update"
    },
    list: {
      name: "globalCoordinator.list.name",
      email: "globalCoordinator.list.email",
      phoneNumber: "globalCoordinator.list.phoneNumber",
      organization: "globalCoordinator.list.organization",
      removalConfirmation: "globalCoordinator.list.removalConfirmation"
    },
  },
  nationalSocietyUser: {
    title: "nationalSocietyUser.title",
    addNew: "nationalSocietyUser.addNew",
    addSuccess: "nationalSocietyUser.addSuccess",
    addExisting: "nationalSocietyUser.addExisting",
    form: {
      creationTitle: "nationalSocietyUser.form.creationTitle",
      addExistingTitle: "nationalSocietyUser.form.addExistingTitle",
      addExistingDescription: "nationalSocietyUser.form.addExistingDescription",
      editionTitle: "nationalSocietyUser.form.editionTitle",
      name: "nationalSocietyUser.form.name",
      role: "nationalSocietyUser.form.role",
      decadeOfBirth: "nationalSocietyUser.form.decadeOfBirth",
      sex: "nationalSocietyUser.form.sex",
      email: "nationalSocietyUser.form.email",
      project: "nationalSocietyUser.form.project",
      phoneNumber: "nationalSocietyUser.form.phoneNumber",
      additionalPhoneNumber: "nationalSocietyUser.form.additionalPhoneNumber",
      organization: "nationalSocietyUser.form.organization",
      create: "nationalSocietyUser.form.create",
      addExisting: "nationalSocietyUser.form.addExisting",
      update: "nationalSocietyUser.form.update"
    },
    list: {
      name: "nationalSocietyUser.list.name",
      email: "nationalSocietyUser.list.email",
      phoneNumber: "nationalSocietyUser.list.phoneNumber",
      role: "nationalSocietyUser.list.role",
      project: "nationalSocietyUser.list.project",
      removalConfirmation: "nationalSocietyUser.list.removalConfirmation",
      headManager: "nationalSocietyUser.list.headManager"
    },
    messages:{
      creationSuccessful: "nationalSocietyUser.messages.creationSuccessful",
    }
  },
  dataCollector: {
    title: "dataCollectors.title",
    addNew: "dataCollectors.addNew",
    form: {
      creationTitle: "dataCollectors.form.creationTitle",
      editionTitle: "dataCollectors.form.editionTitle",
      name: "dataCollectors.form.name",
      displayName: "dataCollectors.form.displayName",
      sex: "dataCollectors.form.sex",
      phoneNumber: "dataCollectors.form.phoneNumber",
      additionalPhoneNumber: "dataCollectors.form.additionalPhoneNumber",
      latitude: "dataCollectors.form.latitude",
      longitude: "dataCollectors.form.longitude",
      create: "dataCollectors.form.create",
      update: "dataCollectors.form.update",
      birthYearGroup: "dataCollectors.form.birthYearGroup",
      supervisor: "dataCollectors.form.supervisor",
      village: "dataCollectors.form.village",
      district: "dataCollectors.form.district",
      region: "dataCollectors.form.region",
      zone: "dataCollectors.form.zone"
    },
    list: {
      name: "dataCollectors.list.name",
      displayName: "dataCollectors.list.displayName",
      phoneNumber: "dataCollectors.list.phoneNumber",
      sex: "dataCollectors.list.sex",
      location: "dataCollectors.list.location",
      status: "dataCollectors.list.status",
      removalConfirmation: "dataCollectors.list.removalConfirmation"
    },
    constants: {
      sex: {
        male: "dataCollectors.sex.male",
        female: "dataCollectors.sex.female",
        other: "dataCollectors.sex.other",
      }
    }
  },
  form: {
    cancel: "form.cancel"
  },
  user: {
    logout: "user.logout",
    verifyEmail: {
      setPassword: "user.verifyEmail.setPassword",
      welcome: "user.verifyEmail.welcome",
      signIn: "user.verifyEmail.signIn",
      password: "user.verifyEmail.password",
      failed: "user.verifyEmail.failed"
    },
    registration: {
      passwordTooWeak: "user.registration.passwordTooWeak"
    },
    resetPassword: {
      success: "user.resetPassword.success",
      failed: "user.resetPassword.failed",
      enterEmail: "user.resetPassword.enterEmail",
      emailAddress: "user.resetPassword.emailAddress",
      submit: "user.resetPassword.submit",
      enterNewPassword: "user.resetPassword.enterNewPassword",
      notFound: "user.resetPassword.notFound"
    }
  },
  validation: {
    invalidPhoneNumber: "validation.invalidPhoneNumber",
    fieldRequired: "validation.fieldRequired",
    tooShortString: "validation.tooShortString",
    tooLongString: "validation.tooLongString",
    invalidEmail: "validation.invalidEmail",
    invalidInteger: "validation.invalidInteger",
    phoneNumberInvalid: "validation.phoneNumberInvalid",
    invalidModuloTen: "validation.invalidModuloTen"
  },
  headManagerConsents: {
    title: "headManagerConsents.title",
    consentText: "headManagerConsents.consentText",
    nationalSociety: "headManagerConsents.nationalSociety",
    nationalSocieties: "headManagerConsents.nationalSocieties",
    agreeToContinue: "headManagerConsents.agreeToContinue",
    submit: "headManagerConsents.submit",
    iConsent: "headManagerConsents.iConsent",
    setAsHeadManager: "headManagerConsents.setAsHeadManager",
    pendingHeadManager: "headManagerConsents.pendingHeadManager",
    setSuccessfully: "headManagerConsents.setSuccessfully",
  }
};

export const strings = (key, noEditor) => {
  const value = stringList[key];

  if (showKeys && !noEditor) {
    return <StringsEditor stringKey={key} />;
  }

  return value === undefined ? key : value;
}

export function updateStrings(strings) {
  Object.assign(stringList, strings);
}

export function toggleStringsMode() {
  showKeys = !showKeys;
}

export default stringList;
