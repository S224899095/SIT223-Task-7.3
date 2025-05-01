import { HttpEvent, HttpHandler, HttpInterceptor, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, take } from 'rxjs';
import { AccountService } from '../services/account.service';
import { UserModel } from '../Models/userModel';

@Injectable()
export class jwtInterceptor implements HttpInterceptor{

    constructor(private acctService: AccountService){ }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        var currentUser: UserModel | null;
        this.acctService.getCurrentUser().pipe(take(1)).subscribe({
            next: (user: any) => {
                currentUser = user
                if(currentUser != null){
                    req = req.clone({
                        setHeaders: {
                            'Content-Type': 'application/json',
                            'Authorization': `Bearer ${currentUser.token}`
                        }
                    })
                }
            } 
        });

        return next.handle(req);
    }

}
