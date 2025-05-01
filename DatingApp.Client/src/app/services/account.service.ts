import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserModel } from '../Models/userModel';
import { map, Observable, ReplaySubject } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { PresenceService } from './presence.service';

@Injectable({
    providedIn: 'root'
})
export class AccountService {
    
    private baseUrl: string = environment.apiUrl + 'account';
    private currentUserSource: ReplaySubject<UserModel | null> = new ReplaySubject(1);
    private currentUser$: Observable<UserModel | null> = this.currentUserSource.asObservable();

    constructor(private http: HttpClient, private presenceService: PresenceService) { }
    
    getStandarOptions(): any {
        var httpHeaders = new HttpHeaders({
            'Content-Type': 'application/json'
        })
        var output = {
            headers: httpHeaders
        };
        return output;
    }

    login(username: string, password: string): any{
        let url = `${this.baseUrl}/login`
        let options = this.getStandarOptions();
        let body = {
            username: username,
            password: password
        }
        return this.http.post(url, body, options).pipe(
            map((user: any) => {
                if(user){
                    this.setLocalStorageUser(user);
                    this.presenceService.createHubConnection(user);
                }
            })
        );
    }
    
    registerUser(username: string, password: string){
        let url = `${this.baseUrl}/register`
        let options = this.getStandarOptions();
        let body = {
            username: username,
            password: password
        }
        return this.http.post(url, body, options).pipe(
            map((user: any) => {
                if(user){
                    this.setLocalStorageUser(user);
                    this.presenceService.createHubConnection(user);
                }
            })
        );
    }

    register(user: object){
        let url = `${this.baseUrl}/register`
        let options = this.getStandarOptions();
        let body = user;
        return this.http.post(url, body, options).pipe(
            map((user: any) => {
                if(user){
                    this.setLocalStorageUser(user);
                    this.presenceService.createHubConnection(user);
                }
            })
        );
    }

    logout(){
        localStorage.removeItem('user');
        this.currentUserSource.next(null);
        this.presenceService.stopHubConnection();
    }

    setLocalStorageUser(user: UserModel){
        user.roles = [];
        const roles = this.getDecodedToken(user.token).role;
        Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
        
        localStorage.setItem('user', JSON.stringify(user));
        this.currentUserSource.next(user);
    }

    checkLocalStorageUser(){
        var localUser = localStorage.getItem('user');
        if(localUser){
            const user: UserModel = JSON.parse(localUser);
            this.currentUserSource.next(user);
        }
        else{
            this.currentUserSource.next(null);
        }
    }

    getCurrentUser(){
        this.checkLocalStorageUser();
        return this.currentUser$;
    }

    getDecodedToken(token: string){
        return JSON.parse(atob(token.split('.')[1]));
    }
}
