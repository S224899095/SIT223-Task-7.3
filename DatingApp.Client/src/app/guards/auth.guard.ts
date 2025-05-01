import { ActivatedRouteSnapshot, CanActivate, CanActivateFn, GuardResult, MaybeAsync, RouterStateSnapshot } from '@angular/router';
import { map, Observable } from 'rxjs';
import { AccountService } from '../services/account.service';
import { ToastrService } from 'ngx-toastr';
import { Injectable } from '@angular/core';
import { UserModel } from '../Models/userModel';

@Injectable({
    providedIn: 'root'
})

export class AuthGuard implements CanActivate{

    constructor(private accService: AccountService,  private toaster: ToastrService){}

    canActivate(): Observable<boolean> {
        var output = this.accService.getCurrentUser().pipe(
            map((user: UserModel | null) => {
                if(user){
                    return true;
                }
                else{
                    this.toaster.error('you shall not pass...');
                    return false;
                }
            })
        )
        return output;
    }
}