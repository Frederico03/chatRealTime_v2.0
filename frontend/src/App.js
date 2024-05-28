import React from 'react'
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Register from './pages/register';
import Login from './pages/login';
import Chat from './pages/chat';
import SetAvatar from './pages/setAvatar'
export default function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/register" element={<Register />} />
                <Route path="/login" element={<Login />} />
                <Route path="/" element={<Chat />} />
                <Route path="/setAvatar" element={<SetAvatar />} />
            </Routes>
        </BrowserRouter>
    )
} 