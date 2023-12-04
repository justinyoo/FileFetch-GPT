import React from 'react';
import './StartPage.css';
import { CgArrowLongRight } from "react-icons/cg";
import { useNavigate } from 'react-router-dom';

function StartPage() {
    const navigate = useNavigate();

    const redirectFunction = () => {
        navigate("/chat");
    }

    return (
        <div className='position-relative w-100 vh-100'>
            <div class="mainPage">
                <svg data-name="Layer 1" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1200 120" preserveAspectRatio="none">
                    <path d="M985.66,92.83C906.67,72,823.78,31,743.84,14.19c-82.26-17.34-168.06-16.33-250.45.39-57.84,11.73-114,31.07-172,41.86A600.21,600.21,0,0,1,0,27.35V120H1200V95.8C1132.19,118.92,1055.71,111.31,985.66,92.83Z" class="shape-fill"></path>
                </svg>
            </div>
            <div className='position-absolute top-50 start-50 translate-middle shadow-lg w-75 h-75' >
                <div className='w-100 h-100 d-flex align-items-center justify-content-center '>
                    <div className='text-center'>
                        <h1 className='font'>Welcome to FileFetch</h1>
                        <p className='fs-4 fst-italic'>"Chat-Driven data retrieval from uploaded files"</p><br />
                        <button className='btn btn-primary rounded-pill py-3 px-4 fs-4' onClick={redirectFunction}>Get started <CgArrowLongRight size={40} /></button>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default StartPage;
