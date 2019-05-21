import {RouterModule, Routes} from '@angular/router';
import {AdminComponent} from './features/admin/admin.component';
import {NgModule} from '@angular/core';
import {AuthGuard} from './core/guards/auth.guard';
import {LoginComponent} from './features/login/login.component';
import {CaseEditorComponent} from './features/case/case-editor.component';
import {CaseListComponent} from './features/case/case-list.component';
import {RegisterUserComponent} from './features/admin/register-user/register-user.component';
import {AdminGuard} from './core/guards/admin.guard';
import {EditUserComponent} from './features/admin/edit-user/edit-user.component';
import {ResetUserComponent} from './features/admin/reset-user/reset-user.component';
import {SuggestionsComponent} from './features/admin/suggestions/suggestions.component';
import {SuperAdminGuard} from './core/guards/superadmin.guard';
import {HelpComponent} from "./features/help/help.component";
import {MetadataComponent} from "./features/case/metadata.component";
import {MetadataListComponent} from "./features/case/metadata-list.component";

const appRoutes: Routes = [
  {path: '', redirectTo: 'caselist', pathMatch: 'full'},
  // {path: 'home', component: HomeComponent, canActivate: [AuthGuard]},
  {path: 'caseeditor', component: CaseEditorComponent, canActivate: [AuthGuard], data: {animation: 'Caseeditor'}},
  {path: 'metadata', component: MetadataComponent, canActivate: [AuthGuard], data: {animation: 'Caseeditor'}},
  {path: 'caseeditor/:id', component: CaseEditorComponent, canActivate: [AuthGuard], data: {animation: 'Caseeditor'}},
  {path: 'metadata/:id', component: MetadataComponent, canActivate: [AuthGuard], data: {animation: 'Caseeditor'}},
  {path: 'caselist', component: CaseListComponent, canActivate: [AuthGuard], data: {animation: 'Caselist'}},
  {path: 'metalist', component: MetadataListComponent, canActivate: [AuthGuard], data: {animation: 'Caselist'}},
  {path: 'admin', component: AdminComponent, canActivate: [AdminGuard], data: {animation: 'Admin'}},
  {path: 'login', component: LoginComponent},
  {path: 'register', component: RegisterUserComponent, canActivate: [AdminGuard], data: {animation: 'Register'}},
  {path: 'edit', component: EditUserComponent, canActivate: [AuthGuard], data: {animation: 'Register'}},
  {path: 'reset', component: ResetUserComponent, canActivate: [AdminGuard], data: {animation: 'Register'}},
  {path: 'suggestions', component: SuggestionsComponent, canActivate: [SuperAdminGuard], data: {animation: 'Register'}},
  {path: 'help', component: HelpComponent, canActivate: [AuthGuard], data: {animation: 'Register'}}
];

@NgModule({
  imports: [
    RouterModule.forRoot(
      appRoutes,
      {enableTracing: false, useHash: true} // <-- debugging purposes only set to true
    )
  ],
  exports: [
    RouterModule
  ]
})

export class AppRoutingModule {
}

