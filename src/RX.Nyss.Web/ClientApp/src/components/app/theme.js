import { createMuiTheme } from "@material-ui/core/styles";

export const theme = createMuiTheme({
  typography: {
    fontSize: 14,
    fontFamily: ["Poppins", '"Helvetica Neue"', 'Arial'].join(','),
    body1: {
      color: "#333333"
    },
    button: {
      color: "#333333"
    },
    h1: {
      fontSize: "48px",
      color: "#C02C2C",
      fontWeight: "400",
      marginBottom: 20
    },
    h2: {
      fontSize: 22,
      fontWeight: 600,
      margin: "10px 0 30px"
    },
    h3: {
      fontSize: 16,
      fontWeight: 600,
      margin: "10px 0 20px"
    },
    h6: {
      fontSize: 14,
      fontWeight: 600,
      margin: "10px 0 10px"
    }
  },
  palette: {
    primary: {
      light: "#8c9eff",
      main: "#C02C2C",
      dark: "#3d5afe",
      contrastText: "#ffffff"
    },
    secondary: {
      light: "#8c9eff",
      main: "#333333",
      dark: "#3d5afe",
      contrastText: "#ffffff"
    }
  },
  overrides: {
    MuiButton: {
      root: {
        borderRadius: 0,
        padding: "10px 15px",
        textTransform: "none",
        fontSize: 16
      },
      outlinedPrimary: {
        border: "2px solid #C02C2C !important"
      },
      text: {
        textDecoration: "underline"
      },
      startIcon: {
        "@media (max-width: 1280px)": {
          display: "none"
        }
      }
    },
    MuiMenu: {
      paper: {
        maxHeight: 200
      }
    },
    MuiBreadcrumbs: {
      root: {
        fontSize: 16
      },
      separator: {
        paddingBottom: 2
      }
    },
    MuiPaper: {
      root: {
        border: "none",
      },
      elevation1: {
        boxShadow: "none",
        background: '#FAFAFA',
        borderRadius: 0,

      }
    },
    MuiInput: {
      root: {
        fontSize: "1rem",
        border: "1px solid #E4E1E0",
        backgroundColor: "#fff"
      },
      input: {
        padding: "10px",
        '&:-webkit-autofill': {
          transitionDelay: '9999s'
        },
      },
      formControl: {
        marginTop: "30px !important"
      },
      multiline: {
        padding: "10px"
      },
      inputMultiline: {
        padding: "0"
      },
      underline: {
        "&:before": {
          borderBottom: "none !important"
        },
        "&:after": {
          borderBottomColor: "#a0a0a0"
        },
        "&:hover": {
        }
      }

    },
    MuiInputLabel: {
      root: {
        //padding: "5px 10px",
        zIndex: 1
      },
      shrink: {
        //padding: 0,
        color: "#000000 !important",
        transform: "translate(0, 2px);",
        right: 0,
        lineHeight: "inherit",
        overflow: "hidden",
        textOverflow: "ellipsis"
      }
    },
    MuiLink: {
      underlineHover: {
        textDecoration: "underline"
      }
    },
    MuiListItem: {
      root: {
        paddingLeft: 20,
        "&$selected": {
          backgroundColor: "rgba(0, 0, 0, 0.06)",
        },
      }
    },
    MuiListItemText: {
      root: {
        padding: "12px 20px",
        overflow: "hidden",
        textOverflow: "ellipsis"
      },
      primary: {
        fontSize: 16,
        color: "#737373"
      }
    },
    MuiTableCell: {
      root: {
        fontSize: "1rem"
      },
      head: {
        fontWeight: 600,
        borderBottomColor: "#8C8C8C",
      }
    },
    MuiTabs: {
      root: {
        borderBottom: "1px solid #e0e0e0",

      },
      indicator: {
        backgroundColor: "#C02C2C",
        height: 3
      }
    },
    MuiCard: {
      root: {
        border: "2px solid #f3f3f3"
      }
    },
    MuiCardHeader: {
      title: {
        fontSize: "16px",
        fontWeight: "bold"
      }
    },
    MuiTab: {
      root: {
        fontSize: "1rem !important"
      },
      textColorInherit: {
        opacity: 1
      }
    },
    MuiTreeItem: {
      root: {
        "&:focus": {
          backgroundColor: "inherit"
        }
      },
      content: {
        padding: "8px",
        background: "inherit !important"
      }
    },
    MuiExpansionPanel: {
      root: {
        border: "2px solid #f3f3f3"
      }
    },
    MuiExpansionPanelActions: {
      root: {
        padding: "15px 20px"
      }
    },
    MuiDivider: {
      root: {
        backgroundColor: "#f3f3f3"
      }
    },
    MuiRadio: {
      colorSecondary: {
        '&$checked': {
          color: '#C02C2C'
        }
      }
    },
    MuiTableSortLabel: {
      root: {
        verticalAlign: "unset"
      }
    }
  },
});
