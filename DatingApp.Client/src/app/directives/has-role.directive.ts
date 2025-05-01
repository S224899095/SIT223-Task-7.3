import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../services/account.service';
import { take } from 'rxjs';
import { UserModel } from '../Models/userModel';

@Directive({
    selector: '[appHasRole]',
    standalone: true
})
export class HasRoleDirective implements OnInit {

    @Input() appHasRole: string[] = [];
    user?: UserModel;

    constructor(private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>, private accountService: AccountService) {
        this.accountService.getCurrentUser().pipe(take(1)).subscribe( user => {
            this.user = user!;
        })
    }

    ngOnInit(): void {
        if (!this.user?.roles || this.user == null) {
            this.viewContainerRef.clear();
            return;
        }

        if (this.user.roles.some(r => this.appHasRole.includes(r))) {
            this.viewContainerRef.createEmbeddedView(this.templateRef);
        }
        else{
            this.viewContainerRef.clear();
        }
    }

}
