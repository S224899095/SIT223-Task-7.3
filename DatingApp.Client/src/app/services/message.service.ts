import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { getPaginatedResults, getPaginationHeaders } from './paginationHelpers';
import { MessageModel } from '../Models/message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { UserModel } from '../Models/userModel';
import { BehaviorSubject, take } from 'rxjs';
import { GroupModel } from '../Models/groupModel';

@Injectable({
    providedIn: 'root'
})
export class MessageService {

    baseUrl = environment.apiUrl + 'messages';
    hubUrl = environment.hubUrl + 'messages';
    private hubConnection?: HubConnection;
    private messageThreadSource = new BehaviorSubject<MessageModel[]>([]);
    messageThread$ = this.messageThreadSource.asObservable();

    constructor(private http: HttpClient) { }

    createHubConnection(user: UserModel, otherUser: string) {
        var hubBuilder = new HubConnectionBuilder();
        this.hubConnection = hubBuilder
            .withUrl(`${this.hubUrl}?user=${otherUser}`, { accessTokenFactory: () => user.token })
            .withAutomaticReconnect()
            .build();
        this.hubConnection.start().catch(error => console.log(error));

        // listening for messages
        this.hubConnection.on('ReceivedMessageThread', messages => {
            this.messageThreadSource.next(messages);
        });

        // listening for new messages
        this.hubConnection.on('NewMessage', message => {
            this.messageThread$.pipe(take(1)).subscribe(messages => this.messageThreadSource.next([...messages, message]));
        });

        // listening for updated members in a group chat
        this.hubConnection.on('UpdatedGroup', (group: GroupModel) => {
            if (group.connections.some(x => x.username === otherUser)) {
                this.messageThread$.pipe(take(1)).subscribe(messages => {
                    messages.forEach(message => {
                        if (!message.dateRead) {
                            message.dateRead = new Date(Date.now())
                        }
                    });

                    this.messageThreadSource.next([...messages])
                });
            }
        });
    }

    stopHubConnection() {
        if (this.hubConnection) {
            this.hubConnection?.stop();
        }
    }

    getMessages(pageNumber: number, pageSize: number, container: string) {
        let params = getPaginationHeaders(pageNumber, pageSize);
        params = params.append('Container', container);
        var url = `${this.baseUrl}`;
        return getPaginatedResults<MessageModel[]>(url, params, this.http);
    }

    getMessageThread(username: string) {
        var url = `${this.baseUrl}/thread/${username}`;
        var output = this.http.get<MessageModel[]>(url);
        return output;
    }

    async sendMessage(username: string, content: string) {
        return this.hubConnection?.invoke('SendMessage', { RecipientUsername: username, content }).catch(error => console.log(error));
    }

    deleteMessage(id: number) {
        var url = `${this.baseUrl}/${id}`;
        return this.http.delete(url)
    }
}
