import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserModel } from '../Models/userModel';
import { AccountService } from './account.service';
import { environment } from '../../environments/environment.development';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    private baseUrl: string = environment.apiUrl + 'user';
    private user?: UserModel;

    constructor(private http: HttpClient, private accService: AccountService) {
        this.accService.getCurrentUser().subscribe({
            next: (data: any) => this.user = data
        });
    }

    private getStandarOptions(): any {
        var httpHeaders = new HttpHeaders({
            'Content-Type': 'application/json',
            'Authentication': 'Bearer ' + this.user?.token
        })
        var output = {
            headers: httpHeaders
        };
        return output;
    }

    getAll() {
        var url = `${this.baseUrl}`;
        return this.http.get<UserModel[]>(url);
    }

    get(id: number): any {
        var url = `${this.baseUrl}/${id}`;
        return this.http.get<UserModel>(url)
    }

    update(student: UserModel): any {
        var url = `${this.baseUrl}/${student.id}`;
        const body = JSON.stringify(student);
        return this.http.put<UserModel>(url, body);
    }

    create(student: UserModel): any {
        var url = `${this.baseUrl}`;
        const body = JSON.stringify(student);
        return this.http.post<UserModel>(url, body);
    }
}
