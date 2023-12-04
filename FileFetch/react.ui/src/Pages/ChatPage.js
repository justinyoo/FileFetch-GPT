import React, { useState,useRef,useEffect } from 'react';
import axios from 'axios';
import { HiArrowNarrowLeft } from "react-icons/hi";
import { useNavigate } from 'react-router-dom';
import Toaster from '../Components/Toast/Toaster';

const ChatPage = () => {
    const [messages, setMessages] = useState([{ text: 'Hello user! How can I help you today?', sender: 'server' }]);
    const [showToast, setShowToast] = useState(false);
    const [toastMessage, setToastMessage] = useState('');
    const [newMessage, setNewMessage] = useState('');
    const [isButtonEnabled, setIsButtonEnabled] = useState(false);
    const [isWarning, setIsWarning] = useState(false);
    const chatContainerRef = useRef();
    const navigate = useNavigate();
    const [selectedFile, setSelectedFile] = useState(null);
    const fileInputRef = useRef(null);

    const handleFileChange = (event) => {
        const file = event.target.files[0];
        setSelectedFile(file);
        setIsButtonEnabled(!!file);
    };

    const handleButtonClick = () => {
        fileInputRef.current.click();
    };

    const redirectFunction = () => {
        navigate("/");
    }

    const handleSendMessage = async () => {
        if (newMessage.trim() === '') return;

        const updatedMessages = [...messages, { text: newMessage, sender: 'user' }];
        setMessages(updatedMessages);
        setNewMessage('');
        setMessages([...updatedMessages, { text: 'Typing...', sender:'server' }]);

        try {
            const formData = new FormData();
            formData.append('file', selectedFile);
            formData.append('chatMessage', newMessage);
            const response = await axios.post('/api/ai/chat', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });
            const serverResponse = { text: response.data, sender: 'server' };
            setMessages([...updatedMessages, serverResponse]);
        } catch (error) {
            if (error.response && error.response.status === 500) {
                setIsWarning(false);
                setToastMessage('Error!!! Some error occured in server.');
                setMessages([...updatedMessages, { text: 'Error while processing your request!', sender: 'server' }]);
                setShowToast(true);
            }
            else if (error.response && error.response.status === 400) {
                setIsWarning(true);
                setToastMessage('Warning!!! This file format is not supported.');
                setMessages([...updatedMessages, { text: 'Error while processing your request!', sender: 'server' }]);
                setShowToast(true);
            }
        }
        scrollToTop();
    };

    const handleKeyDown = (e) => {
        if (e.key === 'Enter') {
            if (!isButtonEnabled) {
                setIsWarning(true);
                setToastMessage('Warning!!! You need to select a file first');
                setShowToast(true);
                return;
            }
            else {
                e.preventDefault();
                handleSendMessage()
            }
            ;
        }
    }

    const scrollToBottom = () => {
        chatContainerRef.current.scrollTop = chatContainerRef.current.scrollHeight;
    };

    useEffect(() => {
        
        scrollToBottom();
        if (showToast) {
            const timer = setTimeout(() => {
                setShowToast(false);
            }, 3000);

            return () => clearTimeout(timer);
        }
    }, [messages, showToast]);

    const scrollToTop = () => {
        chatContainerRef.current.scrollTop = 0;
    };

    return (
        <section className='bg-light w-100 vh-100 m-0'>
            <div className="w-100 vh-100">
                <div className="w-100 h-100">
                    <div className="w-100 h-100 col-md-8 col-lg-6 col-xl-4">
                        <div className="card w-100 h-100 d-flex">
                            <div className="card-header d-flex align-items-center p-2  text-white border-bottom-0" style={{ background: '#7F00FF', height: '5vh' }}>
                                <div className='ms-2' >
                                    <HiArrowNarrowLeft size={30} onClick={redirectFunction } />
                                </div>
                                <div className='w-100 text-center'>
                                    <p className="mb-0 fw-bold ">FileFetch</p>
                                </div>
                            </div>
                            <div className="w-100 d-flex flex-row justify-content-between" style={{height:'95vh'}} >
                                <div className="card col-3 h-100 border-0 px-2" style={{background:'#fbfbfb'}}>
                                    <div className='w-100 mt-2 mx-auto vh-100'>
                                        <div className='w-100 h-22 border bg-light'>
                                            <div className='mx-2 my-3'>
                                                Your File:<br />
                                                <input className='w-100 me-2 py-3 form-control my-2' type='text' id='fileInput' placeholder='Your selected file will be displayed here...' value={selectedFile ? selectedFile.name : ''} readOnly />

                                                <button className='btn text-white py-2' style={{ background: '#7F00FF' }} onClick={handleButtonClick}>Browse</button>
                                                <input
                                                    type="file"
                                                    ref={fileInputRef}
                                                    onChange={handleFileChange}
                                                    style={{ display: 'none' }}
                                                />
                                            </div>
                                        </div>
                                        <div className="mt-4">
                                            <p className="lh-sm"> <strong>Use guidelines</strong> (Do you even need one?):</p>
                                            <ol >
                                                <li className="lh-lg">Select the file you want to extract the data from.
                                                    <br />(Supported formats: <strong>pdf, png/jpg/jpeg</strong>)</li>
                                                <li className="lh-lg">Type your queries and send for the AI to respond.</li>
                                                <p className="lh-lg fst-italic"> <strong>Note:</strong> You can change the file any time you want</p>
                                            </ol>
                                        </div>
                                    </div>
                                </div>
                                <div className='col-9 h-100'>
                                    <div className="card p-5 w-100 border-0" style={{ overflowY: 'scroll', height: '92%' }} ref={chatContainerRef}>

                                        {messages.map((message, index) => (
                                            <div key={index} className={`d-flex flex-row justify-content-${message.sender === 'user' ? 'end' : 'start'} mb-4`}>
                                                {message.sender === 'server' ? (
                                                    <>
                                                        <img src="https://chatgpt.fr/wp-content/uploads/2023/05/Logo-1.svg" alt="avatar 1" style={{ width: '40px', height: '40px' }} />
                                                        <div className={`p-3 ms-2 ${message.sender === 'user' ? 'border' : ''}`} style={{  borderRadius: '15px', backgroundColor: message.sender === 'user' ? '#7F00FF' : '#f4f4f4', color: message.sender === 'user' ? '#FFF' : 'black' }}>
                                                            <p className="small mb-0">{message.text}</p>
                                                        </div>
                                                    </>
                                                ) : (
                                                    <>
                                                            <div className={`p-3 me-2 ${message.sender === 'user' ? 'border' : ''} `} style={{ borderRadius: '15px', backgroundColor: message.sender === 'user' ? '#7F00FF' : '#f4f4f4', color: message.sender === 'user' ? '#FFF' : 'black' }}>
                                                            <p className="small mb-0">{message.text}</p>
                                                        </div>
                                                            <img src="https://www.shutterstock.com/image-vector/user-profile-icon-vector-avatar-600nw-2247726673.jpg" alt="avatar 2" style={{ width: '55px', height: '55px' }} />

                                                    </>
                                                )}
                                            </div>
                                        ))}

                                    </div>

                                    <div className="w-100 border rounded-pill">                                      
                                        <div className='w-100 d-flex'>
                                            <input type='text'
                                                style={{ resize: 'none', outline: 'none',width:'95%' }}
                                                className=" border-0 ps-3 py-3  ms-3 bg-transparent "
                                                id="text"
                                                value={newMessage}
                                                onChange={(e) => setNewMessage(e.target.value)}
                                                onKeyDown={(e) => handleKeyDown(e)}
                                                placeholder='Type your message...'                                             
                                            ></input>
                                            <div><button className="btn mt-2 px-3 me-3 text-white" style={{ backgroundColor: '#7F00FF' }} onClick={handleSendMessage} disabled={!isButtonEnabled}>Send</button></div>
                                        </div>
                                    </div>
                                </div>
                                
                            </div>
                            
                            
                            
                        </div>
                    </div>
                </div>
            </div>
            <Toaster showToast={showToast} message={toastMessage} isWarning={isWarning} />
        </section>
    );
};

export default ChatPage;