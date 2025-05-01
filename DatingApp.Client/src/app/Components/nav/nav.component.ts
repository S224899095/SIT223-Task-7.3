import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../services/account.service';
import { UserModel } from '../../Models/userModel';
import { map, Observable } from 'rxjs';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { HasRoleDirective } from '../../directives/has-role.directive';

@Component({
    selector: 'app-nav',
    standalone: true,
    imports: [FormsModule, CommonModule, RouterLink, HasRoleDirective, RouterLinkActive],
    templateUrl: './nav.component.html',
    styleUrl: './nav.component.css'
})

export class NavComponent {
    user = {
        username: '',
        password: ''
    };
    loginUser?: UserModel;
    currentUser$?: Observable<UserModel>;

    constructor(private accService: AccountService, private router: Router) {
        this.accService = accService;
        this.currentUser$ = this.accService.getCurrentUser().pipe(
            map((data: any) => {
                this.loginUser = data;
                return data;
            })
        );
    }

    public login() {
        if (this.user.username == '') {
            return;
        }
        this.accService.login(this.user.username, this.user.password).subscribe({
            next: () => this.router.navigateByUrl('/members')
        })
    }

    public logout() {
        this.accService.logout();
        this.router.navigateByUrl('/');
    }
}
