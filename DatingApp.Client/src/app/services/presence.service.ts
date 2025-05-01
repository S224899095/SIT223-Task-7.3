import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr'
import { ToastrService } from 'ngx-toastr';
import { UserModel } from '../Models/userModel';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
    providedIn: 'root'
})
export class PresenceService {

    hubUrl = environment.hubUrl + 'presence';
    private hubConnection?: HubConnection;
    private onlineUsersSource = new BehaviorSubject<string[]> ([]);
    onlineUsers$ = this.onlineUsersSource.asObservable();

    constructor(private toastr: ToastrService, private router: Router) { }

    createHubConnection(user: UserModel) {
        var hubBuilder = new HubConnectionBuilder();
        this.hubConnection = hubBuilder
            .withUrl(this.hubUrl, {accessTokenFactory: () => user.token})
            .withAutomaticReconnect()
            .build();
        this.hubConnection.start().catch(error => console.log(error));

        // listening for online users
        this.hubConnection.on('UserIsOnline', username => {
            this.onlineUsers$.pipe(take(1)).subscribe( usernames => {
                this.onlineUsersSource.next([...usernames, username]);
            });
        });

        // listening for offline users
        this.hubConnection.on('UserIsOffline', username => {
            this.onlineUsers$.pipe(take(1)).subscribe( usernames => {
                this.onlineUsersSource.next([...usernames.filter(x => x !== username)]);
            });
        });

        // listening for all online users
        this.hubConnection.on('GetOnlineUsers', (usernames: string[]) => {
            this.onlineUsersSource.next(usernames);
        })

        // listening for new messages
        this.hubConnection.on('NewMessageReceived', (user: UserModel) => {
            this.toastr.info(`New message recieved from ${user.username}`).onTap.subscribe(() => this.router.navigateByUrl(`/members/${user.username}?tab=3`))
        })
    }

    stopHubConnection(){
        this.hubConnection?.stop().catch(error => console.log(error));
    }
}
