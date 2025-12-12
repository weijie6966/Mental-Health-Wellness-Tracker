# Project File Lineup

This repository layout excludes `Platforms`, `bin`, `obj`, `Resources`, and `Properties` as requested.

## Root
- App.xaml / App.xaml.cs
- FodyWeavers.xml / FodyWeavers.xsd
- MauiProgram.cs
- Mental Health Wellness Tracker.csproj
- Mental Health Wellness Tracker.csproj.user

## Models
- AssessmentQuestion.cs
- AssessmentResult.cs
- AssessmentState.cs
- CloudDiaryEntry.cs
- DiaryEntry.cs
- LocalDiaryEntry.cs
- Post.cs
- PostComment.cs
- UserProfile.cs

## Services
- AssessmentRepository.cs
- DiaryRepository.cs
- FirebaseStorageService.cs
- IAssessmentRepository.cs
- IAuthService.cs
- AuthService.cs

## ViewModels
- AnalyticViewModel.cs
- AssessmentDetailViewModel.cs
- AssessmentViewModel.cs
- CommunityViewModel.cs
- ContactUsViewModel.cs
- ForgotPasswordViewModel.cs
- MainViewModel.cs
- ProfileViewModel.cs
- RelayCommand.cs
- SignUpViewModel.cs
- ViewModelBase.cs
- WriteDiaryViewModel.cs

## Views
- AnalyticPage.xaml / AnalyticPage.xaml.cs
- AssessmentDetailPage.xaml / AssessmentDetailPage.xaml.cs
- AssessmentPage.xaml / AssessmentPage.xaml.cs
- CommunityPage.xaml / CommunityPage.xaml.cs
- ContactUsPage.xaml / ContactUsPage.xaml.cs
- ForgotPasswordPage.xaml / ForgotPasswordPage.xaml.cs
- MainPage.xaml / MainPage.xaml.cs
- ProfilePage.xaml / ProfilePage.xaml.cs
- ProfilePictureViewPage.xaml / ProfilePictureViewPage.xaml.cs
- SignUpPage.xaml / SignUpPage.xaml.cs
- SignUpSuccessPage.xaml / SignUpSuccessPage.xaml.cs
- WriteDiaryPage.xaml / WriteDiaryPage.xaml.cs

## ViewModel integration map
- **AnalyticViewModel.cs** → Services: `IAssessmentRepository` to load user assessment history from Firestore/SQLite; Models: `AssessmentResult` and answer data for bar-chart statistics and detail navigation.
- **AssessmentViewModel.cs** → Services: `IAssessmentRepository` (via `AssessmentRepository`) for downloading the question bank and persisting answers; Models: `AssessmentQuestion`, `AssessmentResult`; stores user context from secure storage before saving results with serialized answer data.
- **CommunityViewModel.cs** → Services: `IAssessmentRepository` for diary fetches plus cloud edit/delete/comment/like updates; Models: `CloudDiaryEntry` mapped into `Post`/`PostComment` view data.
- **MainViewModel.cs** → Services: `IAuthService` (via `AuthService`) for login/password validation.
- **ProfileViewModel.cs** → Services: `IAssessmentRepository` for profile load/save; Models: `UserProfile` with persisted avatar path and bio.
- **SignUpViewModel.cs** → Services: `IAuthService` (via `AuthService`) for registration and password validation.
- **WriteDiaryViewModel.cs** → Services: `IAssessmentRepository` for adding diary entries; Models: `LocalDiaryEntry` (mood, content, media paths) plus user metadata from secure storage.
- **ForgotPasswordViewModel.cs** → Services: `IAuthService` (via `AuthService`) to send Firebase reset emails and validate password strength before reset flow.

## Backend integration progress
- **Authentication flows**: login/sign-up/reset rely on `AuthService` for Firebase web API calls, secure storage of user tokens/IDs, and password validation; navigation stacks now instantiate view models directly without DI.
- **Assessments**: question downloads, answer submissions, and analytics all execute through `AssessmentRepository`, serializing answers with user context, syncing to Firestore with the shared API key, and exposing history for charts and detail screens.
- **Diary/community**: diary creation, edits, deletions, likes, and comments route through `AssessmentRepository` to the Firestore REST API; `CloudDiaryEntry`/`Post` models now include Firestore IDs and counters used by the community feed.
- **Profiles**: profile load/save first consult local storage, then Firestore, persisting avatars/bios and marking sync status when cloud updates succeed.
