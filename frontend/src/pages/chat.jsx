import React, { useState, useEffect, useRef } from 'react';
import styled from "styled-components";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { allUsersRoute, host } from '../utils/APIRoutes';
import Contacts from "../components/Contacts";
import Welcome from "../components/Welcome";
import ChatContainer from '../components/ChatContainer';
import { HubConnectionBuilder } from "@microsoft/signalr";

function Chat() {
    const connection = useRef(null);
    const navigate = useNavigate();
    const [contacts, setContacts] = useState([]);
    const [currentUser, setCurrentUser] = useState(undefined);
    const [currentChat, setCurrentChat] = useState(undefined);

    useEffect(() => {
        async function fetchData() {
            if (!localStorage.getItem('chat-app-current-user')) {
                navigate("/login");
            } else {
                setCurrentUser(await JSON.parse(localStorage.getItem("chat-app-current-user")));
                console.log("logado");
            }
        }
        fetchData();
    }, [navigate]);

    useEffect(() => {
        if (currentUser) {
            const newConnection = new HubConnectionBuilder()
                .withUrl(`${host}/chathub`) // URL do seu SignalR Hub
                .withAutomaticReconnect()
                .build();

            newConnection.start()
                .then(() => {
                    // Adicionar usuário ao conectar
                    newConnection.invoke("AddUser", currentUser.id);
                })
                .catch(e => console.log("Connection failed: ", e));

            connection.current = newConnection;

            // Clean up connection on unmount
            return () => {
                if (connection.current) {
                    connection.current.stop();
                }
            };
        }
    }, [currentUser]);

    useEffect(() => {
        async function fetchData() {
            if (currentUser) {
                if (currentUser.isAvatarImageSet) {
                    const data = await axios.get(`${allUsersRoute}/${currentUser.id}`);
                    setContacts(data.data.users);
                } else {
                    navigate("/setAvatar");
                }
            }
        }
        fetchData();
    }, [currentUser, navigate]);

    const handleChatChange = (chat) => {
        setCurrentChat(chat);
    };

    return (
        <Container>
            <div className='container'>
                <Contacts
                    contacts={contacts}
                    currentUser={currentUser}
                    changeChat={handleChatChange}
                />
                {
                    currentChat === undefined ? (
                        <Welcome currentUser={currentUser} />
                    ) : (
                        <ChatContainer
                            currentChat={currentChat}
                            currentUser={currentUser}
                            connection={connection}
                        />
                    )
                }
            </div>
        </Container>
    );
}

const Container = styled.div`
  height: 100vh;
  width: 100vw;
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 1rem;
  align-items: center;
  background-color: #131324;
  .container {
    height: 85vh;
    width: 85vw;
    background-color: #00000076;
    display: grid;
    grid-template-columns: 25% 75%;
    @media screen and (min-width: 720px) and (max-width: 1080px) {
      grid-template-columns: 35% 65%;
    }
  }
`;

export default Chat;
