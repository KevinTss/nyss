import React from "react";
import PropTypes from "prop-types";
import { createFieldComponent } from "./FieldBase";
import TextField from '@material-ui/core/TextField';
import IconButton from '@material-ui/core/IconButton';
import InputAdornment from '@material-ui/core/InputAdornment';
import Visibility from '@material-ui/icons/Visibility';
import VisibilityOff from '@material-ui/icons/VisibilityOff';

const PasswordInput = ({ error, name, label, value, controlProps, customProps }) => {
    const [values, setValues] = React.useState({
        showPassword: false
    });

    const togglePassword = () => {
        setValues({ ...values, showPassword: !values.showPassword });
    };

    const showPasswordComponent = (
        <InputAdornment position="end">
            <IconButton
                aria-label="toggle password visibility"
                onClick={togglePassword}
                onMouseDown={event => event.preventDefault()}
            >
                {values.showPassword ? <Visibility /> : <VisibilityOff />}
            </IconButton>
        </InputAdornment>
    );

    return (
        <TextField
            name={name}
            error={!!error}
            helperText={error}
            label={label}
            value={value}
            fullWidth
            InputLabelProps={{ shrink: true }}
            InputProps={{
                ...controlProps,
                type: values.showPassword ? "text" : "password",
                endAdornment: showPasswordComponent
            }}
            inputProps={{ ...customProps }}
        />
    );
};

PasswordInput.propTypes = {
    label: PropTypes.string,
    controlProps: PropTypes.object,
    value: PropTypes.string,
    name: PropTypes.string,
    error: PropTypes.string
};

export const PasswordInputField = createFieldComponent(PasswordInput);
export default PasswordInputField;
