mergeInto(LibraryManager.library, {
  OpenModal: function (imageUrl, header, details) {
    window.openModal(
      Pointer_stringify(imageUrl),
      Pointer_stringify(header),
      Pointer_stringify(details)
    );
  },
});

// mergeInto(LibraryManager.library, {
//   OpenModal: function (imageUrl) {
//     window.openModal(Pointer_stringify(imageUrl));
//   },
// });
