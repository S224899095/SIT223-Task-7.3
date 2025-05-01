import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { UserModel } from '../Models/userModel';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

    private baseUrl: string = environment.apiUrl + 'admin';

  constructor(private http: HttpClient) { }

    getUserWithRoles(){
        let url = `${this.baseUrl}/users-with-roles`;
        return this.http.get<Partial<UserModel[]>>(url);
    }

    updateUserRoles(username: string, roles: string[]){
        let url = `${this.baseUrl}/edit-roles/${username}?roles=${roles}`;
        return this.http.post(url, {});
    }
}
