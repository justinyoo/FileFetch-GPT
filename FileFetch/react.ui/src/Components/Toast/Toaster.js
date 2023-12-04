import React from 'react';
import './Toaster.css';

const Toaster = ({ message, showToast, isWarning }) => {
    const toasterClassName = `toaster ${isWarning ? 'warning' : ''}`;
    return showToast ? <div className={toasterClassName}>{message}</div> : null;
};

export default Toaster;
