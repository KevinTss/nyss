import React from "react";
import * as auth from "../../authentication/auth";
import { Route, Redirect } from "react-router";
import { BaseLayout } from "../layout/BaseLayout";
import { ReactReduxContext } from 'react-redux'

export const AuthRoute = ({ component: Component, roles, computedMatch, ...rest }) => (
  <Route exact {...rest} render={props => {
    return <ReactReduxContext.Consumer>
      {({ store }) => {
        const user = store.getState().appData.user;

        if (!user) {
          auth.setRedirectUrl(window.location.pathname);
          return <Redirect to={auth.loginUrl} />;
        }

        const redirectUrl = auth.getRedirectUrl();

        if (redirectUrl) {
          auth.removeRedirectUrl();
          return <Redirect to={redirectUrl} />;
        }

        if (roles && roles.length && !roles.some(neededRole => user.roles.some(userRole => userRole === neededRole))) {
          return <BaseLayout authError={"Not authorized"}></BaseLayout>;
        }

        store.dispatch({ type: "ROUTE_CHANGED", url: computedMatch.url, path: computedMatch.path, params: computedMatch.params })
        return <Component {...props} />;
      }}
    </ReactReduxContext.Consumer>;
  }
  } />
);
