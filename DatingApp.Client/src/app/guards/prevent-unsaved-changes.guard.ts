import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanDeactivate, CanDeactivateFn, GuardResult, MaybeAsync, RouterStateSnapshot } from '@angular/router';
import { MemberEditComponent } from '../Components/members/member-edit/member-edit.component';
import { ConfirmService } from '../services/confirm.service';
import { Observable } from 'rxjs';


@Injectable({
    providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {

    constructor(private confirmService: ConfirmService){ }

    canDeactivate(component: MemberEditComponent): Observable<boolean> | boolean {
        if(component.editForm?.dirty){
            // return confirm('Are you sure you want to continue? Any changes will be lost');
            return this.confirmService.confirm();
        }
        return true;
    }

}