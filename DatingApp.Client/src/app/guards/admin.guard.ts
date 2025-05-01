import { ActivatedRouteSnapshot, CanActivate, CanActivateFn, GuardResult, MaybeAsync, RouterStateSnapshot } from '@angular/router';
import { AccountService } from '../services/account.service';
import { ToastrService } from 'ngx-toastr';
import { map, Observable } from 'rxjs';
import { UserModel } from '../Models/userModel';
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})

export class adminGuard implements CanActivate {

    constructor(private accountService: AccountService, private toastr: ToastrService){ }

    canActivate(): Observable<boolean> {
        return this.accountService.getCurrentUser().pipe(
            map((user : UserModel | null) => {
                if(user?.roles.includes('Admin') || user?.roles.includes('Moderator')){
                    return true;
                }

                this.toastr.error('You cannot enter this section...');
                return false;
            })
        )
    }
}
