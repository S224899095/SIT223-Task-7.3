import { Routes } from '@angular/router';
import { MemberDetailsComponent } from './Components/members/member-details/member-details.component';
import { AuthGuard } from './guards/auth.guard';
import { TestErrorsComponent } from './Components/Errors/test-errors/test-errors.component';
import { NotFoundComponent } from './Components/Errors/not-found/not-found.component';
import { ServerErrorComponent } from './Components/Errors/server-error/server-error.component';
import { PreventUnsavedChangesGuard } from './guards/prevent-unsaved-changes.guard';
import { ListsComponent } from './Components/lists/lists.component';
import { MessagesComponent } from './Components/messages/messages.component';
import { MemberListComponent } from './Components/members/member-list/member-list.component';
import { MemberEditComponent } from './Components/members/member-edit/member-edit.component';
import { MembermemberDetailedResolver } from './Resolvers/member-detailed.resolver';
import { AdminPanelComponent } from './Components/admin/admin-panel/admin-panel.component';
import { adminGuard } from './guards/admin.guard';
import { HomeComponent } from './Components/home/home.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent },
            { path: 'members/:username', component: MemberDetailsComponent, resolve: { member: MembermemberDetailedResolver } },
            { path: 'member/edit', component: MemberEditComponent, canDeactivate: [PreventUnsavedChangesGuard] },
            { path: 'lists', component: ListsComponent },
            { path: 'messages', component: MessagesComponent },
            { path: 'admin', component: AdminPanelComponent, canActivate: [adminGuard] },
        ]
    },
    { path: 'not-found', component: NotFoundComponent },
    { path: 'server-error', component: ServerErrorComponent },
    { path: "errors", component: TestErrorsComponent },
    { path: '**', component: NotFoundComponent, pathMatch: 'full' }
];
